"""Run with Blender --background --python Tools/blender_smoke.py -- <repo root>."""
import bpy
import json
import math
import sys
from pathlib import Path
from mathutils import Vector

root = Path(sys.argv[sys.argv.index('--') + 1])
source = root / 'SourceArt' / 'Smoke'
exports = root / 'Game' / 'Assets' / 'Wings' / 'Art' / 'Smoke'
artifacts = root / 'Artifacts'
for folder in (source, exports, artifacts):
    folder.mkdir(parents=True, exist_ok=True)
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
scene = bpy.context.scene
scene.unit_settings.system = 'METRIC'
scene.unit_settings.scale_length = 1.0
bpy.ops.mesh.primitive_cube_add(size=1, location=(0, 0, 0.5))
cube = bpy.context.object
cube.name = 'OneMetreReference'
bpy.ops.object.armature_add(location=(0, 0, 0))
rig = bpy.context.object
rig.name = 'ReferenceRig'
bone = rig.pose.bones[0]
bone.rotation_mode = 'XYZ'
group = cube.vertex_groups.new(name=bone.name)
group.add(list(range(len(cube.data.vertices))), 1.0, 'REPLACE')
modifier = cube.modifiers.new('ReferenceSkin', 'ARMATURE')
modifier.object = rig
cube.parent = rig
for frame, angle in ((1, 0), (13, 25), (25, 0)):
    bone.rotation_euler.y = math.radians(angle)
    bone.keyframe_insert(data_path='rotation_euler', frame=frame)
rig.animation_data.action.name = 'ReferenceSway'
scene.frame_start, scene.frame_end = 1, 25
scene.frame_set(1)
bpy.ops.mesh.primitive_cube_add(size=0.18, location=(0, -1.5, 0.5))
forward = bpy.context.object
forward.name = 'ForwardMarker'
forward.parent = rig
bpy.ops.object.select_all(action='DESELECT')
cube.select_set(True)
rig.select_set(True)
forward.select_set(True)
bpy.context.view_layer.objects.active = rig
bpy.ops.export_scene.fbx(filepath=str(exports / 'OneMetreReference.fbx'), use_selection=True,
    object_types={'MESH', 'ARMATURE'}, axis_forward='-Z', axis_up='Y',
    apply_unit_scale=True, add_leaf_bones=False, bake_anim=True,
    bake_anim_use_all_actions=False, bake_anim_use_nla_strips=False)
material = bpy.data.materials.new('WarmReference')
material.diffuse_color = (0.8, 0.48, 0.2, 1)
cube.data.materials.append(material)
bpy.ops.object.camera_add(location=(3, -4, 2.8))
camera = bpy.context.object
camera.rotation_euler = (Vector((0, 0, 0.5)) - camera.location).to_track_quat('-Z', 'Y').to_euler()
scene.camera = camera
bpy.ops.object.light_add(type='AREA', location=(2, -3, 5))
bpy.context.object.data.energy = 600
bpy.context.object.data.size = 4
scene.world.color = (0.18, 0.18, 0.18)
scene.render.engine = 'BLENDER_EEVEE_NEXT'
scene.render.resolution_x, scene.render.resolution_y = 640, 480
scene.render.resolution_percentage = 100
scene.render.image_settings.file_format = 'PNG'
scene.render.filepath = str(artifacts / 'blender-reference.png')
bpy.ops.wm.save_as_mainfile(filepath=str(source / 'reference.blend'))
bpy.ops.render.render(write_still=True)
(artifacts / 'blender-smoke.json').write_text(json.dumps({
    'version': bpy.app.version_string, 'metres': list(cube.dimensions),
    'frames': [1, 25], 'forward_marker_blender': [0, -1.5, 0.5], 'export': str(exports / 'OneMetreReference.fbx'),
    'render': scene.render.filepath, 'result': 'passed'
}, indent=2))
print('WINGS_BLENDER_SMOKE_PASS')

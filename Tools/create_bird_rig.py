"""Rebuild the editable Blender armature/weighted-mesh template from Unity's prototype layout.
No external assets. Bone rest axes are Blender authoring axes; imported clips replace procedural poses.
"""
import bpy
import json
import math
from pathlib import Path

root = Path(__file__).resolve().parents[1]
source = root / 'SourceArt/Bird'
data = json.loads((source / 'PrototypeRig.json').read_text(encoding='utf-8-sig'))
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
bpy.context.scene.unit_settings.system = 'METRIC'
bpy.context.scene.unit_settings.scale_length = 1

def point(v):
    return (v['x'], -v['z'], v['y'])

armature = bpy.data.armatures.new('WingsBirdSkeleton')
rig = bpy.data.objects.new('WingsBirdRig', armature)
bpy.context.collection.objects.link(rig)
bpy.context.view_layer.objects.active = rig
rig.select_set(True)
bpy.ops.object.mode_set(mode='EDIT')
for item in data['bones']:
    bone = armature.edit_bones.new(item['name'])
    bone.head, bone.tail = point(item['head']), point(item['tail'])
for item in data['bones']:
    if item['parent']:
        armature.edit_bones[item['name']].parent = armature.edit_bones[item['parent']]
bpy.ops.object.mode_set(mode='OBJECT')

for index, item in enumerate(data['meshes']):
    mesh = bpy.data.meshes.new(item['name'])
    vertices = [point(v) for v in item['vertices']]
    triangles = item['triangles']
    # Coordinate conversion preserves handedness; Unity's clockwise faces become Blender's reversed winding.
    faces = [(triangles[i+2], triangles[i+1], triangles[i]) for i in range(0,len(triangles),3)]
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(f'{index:02d}_{item["name"]}', mesh)
    bpy.context.collection.objects.link(obj)
    group = obj.vertex_groups.new(name=item['bone'])
    group.add(list(range(len(vertices))), 1.0, 'REPLACE')
    modifier = obj.modifiers.new('BirdArmature', 'ARMATURE')
    modifier.object = rig
    obj.parent = rig
    material = bpy.data.materials.new(f'Colour_{index}')
    c = item['color']
    material.diffuse_color = (c['r'],c['g'],c['b'],1)
    obj.data.materials.append(material)
    for polygon in mesh.polygons:
        polygon.use_smooth = True

# A small authored clip verifies that wing, leg, tail and head channels survive FBX import.
rig.animation_data_create()
action = bpy.data.actions.new('RigChannelCheck')
rig.animation_data.action = action
for name in ['L_Shoulder','R_Shoulder','L_Knee','R_Knee','Head','Tail']:
    bone = rig.pose.bones[name]
    bone.rotation_mode = 'XYZ'
    for frame, value in [(1,0),(15,0.3),(30,0)]:
        bone.rotation_euler = (value,0,0)
        bone.keyframe_insert(data_path='rotation_euler',frame=frame)
bpy.context.scene.frame_start=1
bpy.context.scene.frame_end=30
bpy.context.scene.frame_set(1)
rig['purpose']='Editable prototype skeleton and weighted mesh; replace with species artwork and authored action clips.'
rig['axis_contract']='Unity +Z forward/+Y up; Blender -Y forward/+Z up; metres.'
bpy.ops.wm.save_as_mainfile(filepath=str(source/'BirdRigTemplate.blend'))
output = root/'Game/Assets/Wings/Art/Bird'
output.mkdir(parents=True,exist_ok=True)
bpy.ops.export_scene.fbx(filepath=str(output/'BirdRigTemplate.fbx'),use_selection=False,object_types={'ARMATURE','MESH'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_use_all_actions=False,bake_anim_use_nla_strips=False)
(root/'Artifacts/bird-rig-export.json').write_text(json.dumps({'bones':len(data['bones']),'weightedMeshes':len(data['meshes']),'clip':'RigChannelCheck','result':'exported'},indent=2))

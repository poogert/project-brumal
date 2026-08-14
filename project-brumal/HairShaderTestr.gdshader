// NOTE: Shader automatically converted from Godot Engine 4.5.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, diffuse_burley, specular_schlick_ggx, depth_prepass_alpha;

uniform vec4 albedo : source_color;
uniform sampler2D texture_albedo : source_color, filter_nearest_mipmap, repeat_enable;
uniform ivec2 albedo_texture_size;
uniform float point_size : hint_range(0.1, 128.0, 0.1);

uniform float roughness : hint_range(0.0, 1.0);
uniform sampler2D texture_metallic : hint_default_white, filter_nearest_mipmap, repeat_enable;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_roughness : hint_roughness_r, filter_nearest_mipmap, repeat_enable;

uniform float specular : hint_range(0.0, 1.0, 0.01);
uniform float metallic : hint_range(0.0, 1.0, 0.01);

uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;
uniform sampler2D noise;
uniform sampler2D tex_vtx_25;



void fragment() {
	vec2 base_uv = UV;

	vec4 albedo_tex = texture(texture_albedo, base_uv);
	ALBEDO = albedo.rgb * albedo_tex.rgb;

	float metallic_tex = dot(texture(texture_metallic, base_uv), metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	SPECULAR = specular;

	vec4 roughness_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
	float roughness_tex = dot(texture(texture_roughness, base_uv), roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	ALPHA *= albedo.a * albedo_tex.a;
}
void vertex() {
// Input:26
	float n_out26p0 = TIME;


// FloatConstant:33
	float n_out33p0 = 0.100000;


// FloatOp:32
	float n_out32p0 = n_out26p0 * n_out33p0;


// UVFunc:37
	vec2 n_in37p1 = vec2(1.00000, 1.00000);
	vec2 n_out37p0 = vec2(n_out32p0) * n_in37p1 + UV;


// Texture2D:25
	vec4 n_out25p0 = texture(noise, n_out37p0);
	float n_out25p1 = n_out25p0.r;
	float n_out25p2 = n_out25p0.g;
	float n_out25p3 = n_out25p0.b;


// FloatConstant:46
	float n_out46p0 = 0.200000;


// FloatOp:43
	float n_out43p0 = n_out25p1 * n_out46p0;


// FloatConstant:45
	float n_out45p0 = 0.9975000;


// FloatOp:42
	float n_out42p0 = n_out43p0 + n_out45p0;


// Input:4
	vec3 n_out4p0 = VERTEX;
	float n_out4p1 = n_out4p0.r;
	float n_out4p2 = n_out4p0.g;
	float n_out4p3 = n_out4p0.b;


// FloatOp:41
	float n_out41p0 = n_out42p0 * n_out4p1;


// FloatConstant:44
	float n_out44p0 = 0.0050000;


// FloatOp:40
	float n_out40p0 = n_out25p2 * n_out44p0;


// FloatOp:39
	float n_out39p0 = n_out40p0 + n_out45p0;


// FloatOp:38
	float n_out38p0 = n_out39p0 * n_out4p2;


// FloatOp:31
	float n_out31p0 = n_out25p3 * n_out46p0;


// FloatOp:35
	float n_out35p0 = n_out31p0 + n_out45p0;


// FloatOp:34
	float n_out34p0 = n_out35p0 * n_out4p3;


// VectorCompose:7
	vec3 n_out7p0 = vec3(n_out41p0, n_out38p0, n_out34p0);


// Output:0
	VERTEX = n_out7p0;


}




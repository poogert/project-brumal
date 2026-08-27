shader_type spatial;
render_mode cull_back, blend_mix;

// Set zoom to the MESH'S SIZE IN METRES: the volume samples object space (not a 0..1
// screen), so that one value normalises both the pattern scale and the animation speed
// to what the Kaleidos editor shows. Left at 1.0 a large mesh looks small-scaled and slow.
group_uniforms Transform;
uniform float zoom = 1.0;
uniform vec2 pan = vec2(0.0);
uniform float rotation = 0.0;
group_uniforms Style;
uniform vec4 tint : source_color = vec4(1.0);
uniform float cellSize = 3.0;
group_uniforms Surface;
uniform bool lit = false;
uniform float glow : hint_range(0.0, 5.0) = 1.0;

varying vec3 obj_pos;
const float MAT_ROUGHNESS = 0.5;
const float MAT_METALLIC = 0.0;



#define spinPhase 0.0
const float jitter = 1.1;
const float minkowskiP = 1.8;
const float starPoints = 6.0;
const float starDepth = 0.0;
const float cellStretch = 1.2;
const float cellAngle = 0.0;
const float cellRadius = 1.6;
const float smoothK = 0.0;
const float octaves = 2.0;
const float gain = 1.05;

const vec4 PALETTE[110] = vec4[110](
  vec4(0.0,0.0886473,0.485999,1.0),
  vec4(0.0,0.0933003,0.489835,1.0),
  vec4(0.0,0.10025,0.495588,1.0),
  vec4(0.0,0.107165,0.501342,1.0),
  vec4(0.0,0.114048,0.507095,1.0),
  vec4(0.0,0.120899,0.512849,1.0),
  vec4(0.0,0.127719,0.518603,1.0),
  vec4(0.0,0.134511,0.524356,1.0),
  vec4(0.0,0.141274,0.53011,1.0),
  vec4(0.0,0.148011,0.535863,1.0),
  vec4(0.0,0.154723,0.541617,1.0),
  vec4(0.0,0.161409,0.547371,1.0),
  vec4(0.0,0.168072,0.553124,1.0),
  vec4(0.0,0.174712,0.558878,1.0),
  vec4(0.0,0.18133,0.564631,1.0),
  vec4(0.0,0.187926,0.570385,1.0),
  vec4(0.0,0.194503,0.576138,1.0),
  vec4(0.0,0.201059,0.581892,1.0),
  vec4(0.0,0.207597,0.587646,1.0),
  vec4(0.0,0.214116,0.593399,1.0),
  vec4(0.0,0.220618,0.599153,1.0),
  vec4(0.0,0.227102,0.604906,1.0),
  vec4(0.0,0.23357,0.61066,1.0),
  vec4(0.0,0.240022,0.616414,1.0),
  vec4(0.0,0.246459,0.622167,1.0),
  vec4(0.0,0.25288,0.627921,1.0),
  vec4(0.0,0.259288,0.633674,1.0),
  vec4(0.0,0.265681,0.639428,1.0),
  vec4(0.0,0.272061,0.645182,1.0),
  vec4(0.0,0.278427,0.650935,1.0),
  vec4(0.0,0.284781,0.656689,1.0),
  vec4(0.0,0.291123,0.662442,1.0),
  vec4(0.0,0.297452,0.668196,1.0),
  vec4(0.0,0.30377,0.673949,1.0),
  vec4(0.0,0.310077,0.679703,1.0),
  vec4(0.0,0.316373,0.685457,1.0),
  vec4(0.0,0.322659,0.69121,1.0),
  vec4(0.0,0.328934,0.696964,1.0),
  vec4(0.0,0.335199,0.702717,1.0),
  vec4(0.0,0.341454,0.708471,1.0),
  vec4(0.0,0.3477,0.714225,1.0),
  vec4(0.0,0.353937,0.719978,1.0),
  vec4(0.0,0.360165,0.725732,1.0),
  vec4(0.00128325,0.366384,0.731485,1.0),
  vec4(0.00795137,0.372595,0.737239,1.0),
  vec4(0.0146031,0.378798,0.742993,1.0),
  vec4(0.0212389,0.384992,0.748746,1.0),
  vec4(0.0278591,0.391179,0.7545,1.0),
  vec4(0.0344642,0.397359,0.760253,1.0),
  vec4(0.0410545,0.403531,0.766007,1.0),
  vec4(0.0476305,0.409695,0.77176,1.0),
  vec4(0.0541925,0.415853,0.777514,1.0),
  vec4(0.0607409,0.422004,0.783268,1.0),
  vec4(0.0672759,0.428149,0.789021,1.0),
  vec4(0.073798,0.434286,0.794775,1.0),
  vec4(0.0803074,0.440418,0.800528,1.0),
  vec4(0.0868045,0.446543,0.806282,1.0),
  vec4(0.0932896,0.452663,0.812036,1.0),
  vec4(0.0997629,0.458776,0.817789,1.0),
  vec4(0.106225,0.464884,0.823543,1.0),
  vec4(0.112675,0.470986,0.829296,1.0),
  vec4(0.119115,0.477083,0.83505,1.0),
  vec4(0.125544,0.483174,0.840804,1.0),
  vec4(0.131963,0.48926,0.846557,1.0),
  vec4(0.138371,0.495341,0.852311,1.0),
  vec4(0.14477,0.501417,0.858064,1.0),
  vec4(0.151159,0.507488,0.863818,1.0),
  vec4(0.157538,0.513555,0.869571,1.0),
  vec4(0.163908,0.519616,0.875325,1.0),
  vec4(0.170269,0.525674,0.881079,1.0),
  vec4(0.17662,0.531726,0.886832,1.0),
  vec4(0.182964,0.537775,0.892586,1.0),
  vec4(0.189298,0.543819,0.898339,1.0),
  vec4(0.195625,0.549859,0.904093,1.0),
  vec4(0.201943,0.555895,0.909847,1.0),
  vec4(0.208253,0.561927,0.9156,1.0),
  vec4(0.214556,0.567955,0.921354,1.0),
  vec4(0.220851,0.573979,0.927107,1.0),
  vec4(0.227138,0.579999,0.932861,1.0),
  vec4(0.233418,0.586016,0.938615,1.0),
  vec4(0.239691,0.592029,0.944368,1.0),
  vec4(0.245956,0.598039,0.950122,1.0),
  vec4(0.252215,0.604045,0.955875,1.0),
  vec4(0.258467,0.610048,0.961629,1.0),
  vec4(0.264713,0.616048,0.967382,1.0),
  vec4(0.270952,0.622044,0.973136,1.0),
  vec4(0.277185,0.628037,0.97889,1.0),
  vec4(0.283411,0.634027,0.984643,1.0),
  vec4(0.289631,0.640014,0.990397,1.0),
  vec4(0.295845,0.645998,0.99615,1.0),
  vec4(0.302119,0.652119,1.0,1.0),
  vec4(0.30852,0.65852,1.0,1.0),
  vec4(0.314922,0.664922,1.0,1.0),
  vec4(0.321324,0.671324,1.0,1.0),
  vec4(0.327726,0.677726,1.0,1.0),
  vec4(0.334128,0.684128,1.0,1.0),
  vec4(0.34053,0.69053,1.0,1.0),
  vec4(0.346932,0.696932,1.0,1.0),
  vec4(0.353334,0.703334,1.0,1.0),
  vec4(0.359735,0.709735,1.0,1.0),
  vec4(0.366137,0.716137,1.0,1.0),
  vec4(0.372539,0.722539,1.0,1.0),
  vec4(0.378941,0.728941,1.0,1.0),
  vec4(0.385343,0.735343,1.0,1.0),
  vec4(0.391745,0.741745,1.0,1.0),
  vec4(0.398147,0.748147,1.0,1.0),
  vec4(0.404549,0.754549,1.0,1.0),
  vec4(0.41095,0.76095,1.0,1.0),
  vec4(0.417352,0.767352,1.0,1.0),
  vec4(0.42162,0.77162,1.0,1.0)
);
vec4 paletteLookup(float x){
  int i = clamp(int(clamp(x,0.0,1.0)*256.0),0,255);
  return PALETTE[clamp(int(float(i)/255.0*110.0),0,109)];
}

#define TAU 6.28318530718
#define MAX_OCTAVES 10
#define VOLUME_Z    0.7

vec2 latticeOffset(vec2 cell) {
    float row = floor(cell.y);
    return vec2(0.0);
}

vec3 hash3(vec3 p) {
    p = vec3(dot(p, vec3(127.1, 311.7, 74.7)),
             dot(p, vec3(269.5, 183.3, 246.1)),
             dot(p, vec3(113.5, 271.9, 124.6)));
    return fract(sin(p) * 43758.5453);
}

float metricDist3(vec3 d, float ang) {
    float s = sin(ang), c = cos(ang);
    d.xy = mat2(vec2(c, -s), vec2(s, c)) * d.xy;
    d.x *= cellStretch; d.y /= cellStretch;
    float p  = max(minkowskiP, 0.1);
    float md = pow(pow(abs(d.x), p) + pow(abs(d.y), p) + pow(abs(d.z), p), 1.0 / p);
    return md * (1.0 + starDepth * cos(starPoints * atan(d.y, d.x)));
}

float cellSpin(vec2 cell) {
    float turns = cellAngle / 360.0;
    turns += spinPhase;
    return -turns * TAU;
}

float smin(float a, float b, float k) {
    if (k <= 0.0) return min(a, b);
    float h = clamp(0.5 + 0.5 * (a - b) / k, 0.0, 1.0);
    return mix(a, b, h) - k * h * (1.0 - h);
}

void consider(float d, inout float f1, inout float f2) {
    if      (d < f1) f2 = f1;
    else if (d < f2) f2 = d;
    f1 = smin(f1, d, smoothK);
}

float comboValue(float f1, float f2) {
    float s = 1.0 / max(cellRadius, 0.001);
    f1 *= s; f2 *= s;
    float v;
    v = f1 / max(f2, 0.001);
    v = clamp(v, 0.0, 1.0);

    return clamp(v, 0.0, 1.0);
}

#define CELL_EMPTY_LO 4.0
#define CELL_EMPTY_HI 7.0
#define CELL_EMPTY_VALUE 1.0

float cellFinish(float f1, float f2, bool doCells) {
    float v = comboValue(f1, f2);
    if (doCells) v = mix(v, CELL_EMPTY_VALUE, smoothstep(CELL_EMPTY_LO, CELL_EMPTY_HI, f1));
    
    return v;
}

float cellFieldVol(vec3 P){
  vec3 ip = floor(P), fp = fract(P);
  float f1 = 8.0, f2 = 8.0;
  for (int k = -1; k <= 1; k++) {
    for (int j = -2; j <= 2; j++) {
      for (int i = -2; i <= 2; i++) {
        vec3 g    = vec3(float(i), float(j), float(k));
        vec3 cell = ip + g;
        vec3 rnd  = hash3(cell);
        vec3 pt   = g + vec3(mix(vec2(0.5), rnd.xy, jitter), rnd.z);
        pt.xy    += latticeOffset(cell.xy) * (1.0 - jitter);
        vec3 d    = pt - fp;
        d.z      *= VOLUME_Z;
        consider(metricDist3(d, cellSpin(cell.xy)), f1, f2);
      }
    }
  }
  return cellFinish(f1, f2, false);
}
float cellFBMVol(vec3 p){
  float v = 0.0, a = 0.5, norm = 0.0;
  int oct = int(octaves + 0.5);
  for (int i = 0; i < MAX_OCTAVES; i++) {
    if (i >= oct) break;
    v    += a * cellFieldVol(p);
    norm += a;
    p     = p * 2.0 + vec3(5.3, 1.7, 3.1);
    a    *= gain;
  }
  return v / norm;
}


void vertex(){ obj_pos = VERTEX; }
void fragment(){
  vec3 p = obj_pos;
  float s = sin(rotation), cs = cos(rotation);
  p.xz = mat2(vec2(cs, -s), vec2(s, cs)) * p.xz;
  p = p / max(zoom, 0.0001) + vec3(pan, 0.0);
  float vox = cellSize / 640.0;
  if (cellSize > 1.0) p = floor(p / vox) * vox;
  vec2 pl = p.xz * 6.0;
  float pz = p.y * 6.0;
  float n = clamp(cellFBMVol(vec3(pl, pz)), 0.0, 1.0);
  vec4 c = paletteLookup(n);
  vec3 col = c.rgb * tint.rgb;
  if (lit){
    ALBEDO = col; ROUGHNESS = MAT_ROUGHNESS; METALLIC = MAT_METALLIC;
    EMISSION = col * max(glow - 1.0, 0.0);
  } else {
    ALBEDO = vec3(0.0); EMISSION = col * glow;
  }
  ALPHA = c.a * tint.a;
}
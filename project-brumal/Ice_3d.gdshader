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
uniform float cellSize = 1.0;
group_uniforms Surface;
uniform bool lit = false;
uniform float glow : hint_range(0.0, 5.0) = 1.0;

varying vec3 obj_pos;
const float MAT_ROUGHNESS = 0.5;
const float MAT_METALLIC = 0.0;



#define spinPhase 0.0
const float jitter = 0.7;
const float minkowskiP = 1.6;
const float starPoints = 6.0;
const float starDepth = 0.0;
const float cellStretch = 1.4;
const float cellAngle = 0.0;
const float cellRadius = 0.9;
const float smoothK = 0.0;
const float octaves = 2.0;
const float gain = 0.7;

const vec4 PALETTE[110] = vec4[110](
  vec4(0.0,0.0780833,0.428083,1.0),
  vec4(0.0,0.0823512,0.432351,1.0),
  vec4(0.0,0.0887531,0.438753,1.0),
  vec4(0.0,0.095155,0.445155,1.0),
  vec4(0.0,0.101557,0.451557,1.0),
  vec4(0.0,0.107959,0.457959,1.0),
  vec4(0.0,0.114361,0.464361,1.0),
  vec4(0.0,0.120763,0.470763,1.0),
  vec4(0.0,0.127164,0.477164,1.0),
  vec4(0.0,0.133566,0.483566,1.0),
  vec4(0.0,0.139968,0.489968,1.0),
  vec4(0.0,0.14637,0.49637,1.0),
  vec4(0.0,0.152772,0.502772,1.0),
  vec4(0.0,0.159174,0.509174,1.0),
  vec4(0.0,0.165576,0.515576,1.0),
  vec4(0.0,0.171978,0.521978,1.0),
  vec4(0.0,0.178379,0.528379,1.0),
  vec4(0.0,0.184781,0.534781,1.0),
  vec4(0.0,0.191183,0.541183,1.0),
  vec4(0.0,0.197585,0.547585,1.0),
  vec4(0.0,0.203987,0.553987,1.0),
  vec4(0.0,0.210389,0.560389,1.0),
  vec4(0.0,0.216791,0.566791,1.0),
  vec4(0.0,0.223193,0.573193,1.0),
  vec4(0.0,0.229594,0.579594,1.0),
  vec4(0.0,0.235996,0.585996,1.0),
  vec4(0.0,0.242398,0.592398,1.0),
  vec4(0.0,0.2488,0.5988,1.0),
  vec4(0.0,0.255202,0.605202,1.0),
  vec4(0.0,0.261604,0.611604,1.0),
  vec4(0.0,0.268006,0.618006,1.0),
  vec4(0.0,0.274408,0.624408,1.0),
  vec4(0.0,0.28081,0.63081,1.0),
  vec4(0.0,0.287211,0.637211,1.0),
  vec4(0.0,0.293613,0.643613,1.0),
  vec4(0.0,0.300015,0.650015,1.0),
  vec4(0.0,0.306417,0.656417,1.0),
  vec4(0.0,0.312819,0.662819,1.0),
  vec4(0.0,0.319221,0.669221,1.0),
  vec4(0.0,0.325623,0.675623,1.0),
  vec4(0.0,0.332025,0.682025,1.0),
  vec4(0.0,0.338426,0.688426,1.0),
  vec4(0.0,0.344828,0.694828,1.0),
  vec4(0.00123018,0.35123,0.70123,1.0),
  vec4(0.00763205,0.357632,0.707632,1.0),
  vec4(0.0140339,0.364034,0.714034,1.0),
  vec4(0.0204358,0.370436,0.720436,1.0),
  vec4(0.0268377,0.376838,0.726838,1.0),
  vec4(0.0332396,0.38324,0.73324,1.0),
  vec4(0.0396415,0.389641,0.739641,1.0),
  vec4(0.0460433,0.396043,0.746043,1.0),
  vec4(0.0524452,0.402445,0.752445,1.0),
  vec4(0.0588471,0.408847,0.758847,1.0),
  vec4(0.065249,0.415249,0.765249,1.0),
  vec4(0.0716508,0.421651,0.771651,1.0),
  vec4(0.0780527,0.428053,0.778053,1.0),
  vec4(0.0844546,0.434455,0.784455,1.0),
  vec4(0.0908565,0.440856,0.790856,1.0),
  vec4(0.0972584,0.447258,0.797258,1.0),
  vec4(0.10366,0.45366,0.80366,1.0),
  vec4(0.110062,0.460062,0.810062,1.0),
  vec4(0.116464,0.466464,0.816464,1.0),
  vec4(0.122866,0.472866,0.822866,1.0),
  vec4(0.129268,0.479268,0.829268,1.0),
  vec4(0.13567,0.48567,0.83567,1.0),
  vec4(0.142072,0.492072,0.842072,1.0),
  vec4(0.148473,0.498473,0.848473,1.0),
  vec4(0.154875,0.504875,0.854875,1.0),
  vec4(0.161277,0.511277,0.861277,1.0),
  vec4(0.167679,0.517679,0.867679,1.0),
  vec4(0.174081,0.524081,0.874081,1.0),
  vec4(0.180483,0.530483,0.880483,1.0),
  vec4(0.186885,0.536885,0.886885,1.0),
  vec4(0.193287,0.543287,0.893287,1.0),
  vec4(0.199688,0.549688,0.899688,1.0),
  vec4(0.20609,0.55609,0.90609,1.0),
  vec4(0.212492,0.562492,0.912492,1.0),
  vec4(0.218894,0.568894,0.918894,1.0),
  vec4(0.225296,0.575296,0.925296,1.0),
  vec4(0.231698,0.581698,0.931698,1.0),
  vec4(0.2381,0.5881,0.9381,1.0),
  vec4(0.244502,0.594502,0.944502,1.0),
  vec4(0.250903,0.600903,0.950903,1.0),
  vec4(0.257305,0.607305,0.957305,1.0),
  vec4(0.263707,0.613707,0.963707,1.0),
  vec4(0.270109,0.620109,0.970109,1.0),
  vec4(0.276511,0.626511,0.976511,1.0),
  vec4(0.282913,0.632913,0.982913,1.0),
  vec4(0.289315,0.639315,0.989315,1.0),
  vec4(0.295717,0.645717,0.995717,1.0),
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
    v = f1 * 1.4;
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
  vec2 pl = p.xz * 7.0;
  float pz = p.y * 7.0;
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
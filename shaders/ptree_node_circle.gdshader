shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

float circle(vec2 position, float radius, float feather) {
	return smoothstep(radius, radius + feather, length(position - vec2(0.5)));
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec3 preColour = vec3( circle(UV, 0.45, 0.005) );
	if (preColour.x == 1.0)
		COLOR.a = 0.0;
}

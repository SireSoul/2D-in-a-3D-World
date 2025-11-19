#version 330 core

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform float timeOfDay;   // 0 → 1
uniform float intensity;   // general light boost

out vec4 finalColor;

void main()
{
    vec4 col = texture(texture0, fragTexCoord);

    // --- BASIC DAY/NIGHT BRIGHTNESS ---
    float day = sin(timeOfDay * 6.28318) * 0.5 + 0.5;
    float brightness = mix(0.25, 1.0, day);

    // --- COLOR TINTS ---
    vec3 nightTint = vec3(0.5, 0.6, 1.2); // blue-ish night
    vec3 dayTint   = vec3(1.0, 1.0, 1.0);
    vec3 tint      = mix(nightTint, dayTint, day);

    // --- FOG / DISTANT DARKENING ---
    float fog = pow(1.0 - day, 2.0);  // darker fog at night
    vec3 fogColor = vec3(0.1, 0.1, 0.15);
    float fogStrength = 0.15;

    // --- COMBINE EFFECTS ---
    vec3 color = col.rgb;
    color *= brightness * intensity;
    color *= tint;

    // Fake fog
    color = mix(color, fogColor, fog * fogStrength);

    finalColor = vec4(color, col.a);
}
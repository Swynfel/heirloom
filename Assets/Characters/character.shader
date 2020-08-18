shader_type canvas_item;

uniform vec4 outline : hint_color;
uniform vec4 primary_0 : hint_color;
uniform vec4 primary_1 : hint_color;
uniform vec4 primary_2 : hint_color;
uniform vec4 primary_3 : hint_color;
uniform vec4 secondary_0 : hint_color;
uniform vec4 secondary_1 : hint_color;
uniform vec4 secondary_2 : hint_color;
uniform vec4 skin_0 : hint_color;
uniform vec4 skin_1 : hint_color;
uniform vec4 skin_2 : hint_color;
uniform vec4 skin_3 : hint_color;
uniform vec4 hair_0 : hint_color;
uniform vec4 hair_1 : hint_color;
uniform vec4 hair_2 : hint_color;
uniform vec4 hair_3 : hint_color;
uniform vec4 eyebrows : hint_color;
uniform vec4 eyeball_0 : hint_color;
uniform vec4 eyeball_1 : hint_color;
uniform vec4 iris_0 : hint_color;
uniform vec4 iris_1 : hint_color;
uniform vec4 iris_2 : hint_color;

void fragment() {
    COLOR = texture(TEXTURE,UV);
    int red = int(255f * COLOR.r);
    if(COLOR.a == 1f) {
        switch(red) {
            case 0:
                COLOR = outline;
                break;
            case 103:
                COLOR = primary_0;
                break;
            case 118:
                COLOR = primary_1;
                break;
            case 133:
                COLOR = primary_2;
                break;
            case 156:
                COLOR = primary_3;
                break;
            case 128:
                COLOR = secondary_0;
                break;
            case 195:
                COLOR = secondary_1;
                break;
            case 223:
                COLOR = secondary_2;
                break;
            case 144:
                COLOR = skin_0;
                break;
            case 197:
                COLOR = skin_1;
                break;
            case 210:
                COLOR = skin_2;
                break;
            case 231:
                COLOR = skin_3;
                break;
            case 112:
                COLOR = hair_0;
                break;
            case 148:
                COLOR = hair_1;
                break;
            case 176:
                COLOR = hair_2;
                break;
            case 216:
                COLOR = hair_3;
                break;
            case 39:
                COLOR = eyebrows;
                break;
            case 225:
                COLOR = eyeball_0;
                break;
            case 255:
                COLOR = eyeball_1;
                break;
            case 15:
                COLOR = iris_0;
                break;
            case 31:
                COLOR = iris_1;
                break;
            case 51:
                COLOR = iris_2;
                break;
            default:
                break;
        }
    }
}
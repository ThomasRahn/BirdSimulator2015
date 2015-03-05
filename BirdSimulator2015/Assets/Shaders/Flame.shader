// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:Diffuse,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:33474,y:32712,varname:node_1,prsc:2|diff-2-OUT,emission-2-OUT,alpha-3131-OUT,olwid-6-OUT,olcol-7-A;n:type:ShaderForge.SFN_Lerp,id:2,x:33243,y:32646,varname:node_2,prsc:2|A-3-RGB,B-5-RGB,T-4-OUT;n:type:ShaderForge.SFN_Color,id:3,x:33009,y:32478,ptovrint:False,ptlb:ColorMin,ptin:_ColorMin,varname:node_1930,prsc:2,glob:False,c1:1,c2:0.1568628,c3:0.1568628,c4:1;n:type:ShaderForge.SFN_NormalVector,id:4,x:33003,y:32752,prsc:2,pt:False;n:type:ShaderForge.SFN_Color,id:5,x:32858,y:32551,ptovrint:False,ptlb:ColorMax,ptin:_ColorMax,varname:node_5502,prsc:2,glob:False,c1:1,c2:0.4901961,c3:0.7058824,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6,x:33219,y:32953,ptovrint:False,ptlb:OutlineWidth,ptin:_OutlineWidth,varname:node_4404,prsc:2,glob:False,v1:0.1;n:type:ShaderForge.SFN_Color,id:7,x:33079,y:33041,ptovrint:False,ptlb:OutlineColor,ptin:_OutlineColor,varname:node_878,prsc:2,glob:False,c1:1,c2:0.509804,c3:0.1960784,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:3131,x:33219,y:32825,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_3131,prsc:2,glob:False,v1:0;proporder:3-5-6-7-3131;pass:END;sub:END;*/

Shader "Turrim/Flame" {
    Properties {
        _ColorMin ("ColorMin", Color) = (1,0.1568628,0.1568628,1)
        _ColorMax ("ColorMax", Color) = (1,0.4901961,0.7058824,1)
        _OutlineWidth ("OutlineWidth", Float ) = 0.1
        _OutlineColor ("OutlineColor", Color) = (1,0.509804,0.1960784,1)
        _Alpha ("Alpha", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*_OutlineWidth,1));
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
                return fixed4(float3(_OutlineColor.a,_OutlineColor.a,_OutlineColor.a),0);
            }
            ENDCG
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _ColorMin;
            uniform float4 _ColorMax;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 node_2 = lerp(_ColorMin.rgb,_ColorMax.rgb,i.normalDir);
                float3 diffuse = (directDiffuse + indirectDiffuse) * node_2;
////// Emissive:
                float3 emissive = node_2;
/// Final Color:
                float3 finalColor = diffuse + emissive;
                return fixed4(finalColor,_Alpha);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _ColorMin;
            uniform float4 _ColorMax;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 node_2 = lerp(_ColorMin.rgb,_ColorMax.rgb,i.normalDir);
                float3 diffuse = directDiffuse * node_2;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * _Alpha,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
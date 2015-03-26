// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1058824,fgcg:0.1490196,fgcb:0.2431373,fgca:1,fgde:0.0003,fgrn:0,fgrf:0.01,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:6682,x:33169,y:32738,varname:node_6682,prsc:2|emission-8254-OUT,alpha-5973-OUT;n:type:ShaderForge.SFN_Vector1,id:8254,x:32942,y:32748,varname:node_8254,prsc:2,v1:1;n:type:ShaderForge.SFN_Distance,id:4992,x:32777,y:32998,varname:node_4992,prsc:2|A-6259-Z,B-7010-Z;n:type:ShaderForge.SFN_ObjectPosition,id:6259,x:32588,y:32892,varname:node_6259,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:7010,x:32588,y:33018,varname:node_7010,prsc:2;n:type:ShaderForge.SFN_Slider,id:7904,x:32741,y:33202,ptovrint:False,ptlb:Amount,ptin:_Amount,varname:node_7904,prsc:2,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:5973,x:32963,y:32998,varname:node_5973,prsc:2|A-246-OUT,B-7904-OUT;n:type:ShaderForge.SFN_OneMinus,id:246,x:32762,y:32810,varname:node_246,prsc:2|IN-4992-OUT;proporder:7904;pass:END;sub:END;*/

Shader "Shader Forge/Tether" {
    Properties {
        _Amount ("Amount", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
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
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float _Amount;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD2;
                #else
                    float3 shLight : TEXCOORD2;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float4 objPos = mul ( _Object2World, float4(0,0,0,1) );
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( _Object2World, float4(0,0,0,1) );
/////// Vectors:
////// Lighting:
////// Emissive:
                float node_8254 = 1.0;
                float3 emissive = float3(node_8254,node_8254,node_8254);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,((1.0 - distance(objPos.b,i.posWorld.b))*_Amount));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

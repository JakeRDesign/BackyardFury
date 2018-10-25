// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:10,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|emission-3097-OUT,alpha-7822-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32148,y:32717,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7867647,c2:0.7867647,c3:0.7867647,c4:0.447;n:type:ShaderForge.SFN_Color,id:2578,x:32148,y:32547,ptovrint:False,ptlb:HighlightColor,ptin:_HighlightColor,varname:node_2578,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.7103448,c3:1,c4:1;n:type:ShaderForge.SFN_Vector4Property,id:254,x:31411,y:32747,ptovrint:False,ptlb:HighlightPos,ptin:_HighlightPos,varname:node_254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0.597,v3:0,v4:0;n:type:ShaderForge.SFN_Code,id:2855,x:31691,y:32906,varname:node_2855,prsc:2,code:ZgBsAG8AYQB0ADMAIABkAGkAZgAgAD0AIABoAGkALQB3AG8AOwAKAHIAZQB0AHUAcgBuACAAcABvAHcAKABzAHEAcgB0ACgAKABkAGkAZgAuAHgAKgBkAGkAZgAuAHgAKQArACgAZABpAGYALgB6ACoAZABpAGYALgB6ACkAKwAoAGQAaQBmAC4AeQAqAGQAaQBmAC4AeQApACkALwAoAGQAcwB0AC8AMgAuADAAKQAsAGUAZABnAGUAKQA7AA==,output:0,fname:Function_node_2855,width:319,height:166,input:2,input:2,input:0,input:0,input_1_label:hi,input_2_label:wo,input_3_label:dst,input_4_label:edge|A-254-XYZ,B-5253-XYZ,C-314-OUT,D-102-OUT;n:type:ShaderForge.SFN_Lerp,id:3097,x:32497,y:32734,varname:node_3097,prsc:2|A-2578-RGB,B-1304-RGB,T-1611-OUT;n:type:ShaderForge.SFN_ValueProperty,id:314,x:31411,y:33063,ptovrint:False,ptlb:HighlightDist,ptin:_HighlightDist,varname:node_314,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.4;n:type:ShaderForge.SFN_FragmentPosition,id:5253,x:31411,y:32911,varname:node_5253,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:102,x:31411,y:33143,ptovrint:False,ptlb:HighlightEdge,ptin:_HighlightEdge,varname:node_102,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Clamp01,id:1611,x:32148,y:32903,varname:node_1611,prsc:2|IN-2855-OUT;n:type:ShaderForge.SFN_Lerp,id:7822,x:32497,y:32910,varname:node_7822,prsc:2|A-2578-A,B-1304-A,T-1611-OUT;proporder:1304-254-2578-314-102;pass:END;sub:END;*/

Shader "Shader Forge/LineShader" {
    Properties {
        _Color ("Color", Color) = (0.7867647,0.7867647,0.7867647,0.447)
        _HighlightPos ("HighlightPos", Vector) = (0,0.597,0,0)
        _HighlightColor ("HighlightColor", Color) = (0,0.7103448,1,1)
        _HighlightDist ("HighlightDist", Float ) = 1.4
        _HighlightEdge ("HighlightEdge", Float ) = 2
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+10"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float4 _HighlightColor;
            uniform float4 _HighlightPos;
            float Function_node_2855( float3 hi , float3 wo , float dst , float edge ){
            float3 dif = hi-wo;
            return pow(sqrt((dif.x*dif.x)+(dif.z*dif.z)+(dif.y*dif.y))/(dst/2.0),edge);
            }
            
            uniform float _HighlightDist;
            uniform float _HighlightEdge;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_1611 = saturate(Function_node_2855( _HighlightPos.rgb , i.posWorld.rgb , _HighlightDist , _HighlightEdge ));
                float3 emissive = lerp(_HighlightColor.rgb,_Color.rgb,node_1611);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,lerp(_HighlightColor.a,_Color.a,node_1611));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

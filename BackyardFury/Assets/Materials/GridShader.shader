// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:-10,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|emission-3097-OUT,alpha-150-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:31431,y:32689,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:0.447;n:type:ShaderForge.SFN_Code,id:150,x:32077,y:33098,varname:node_150,prsc:2,code:aQBmACgAZgByAGEAYwAoACgAcABvAHMALgB4ACsAbwBmAGYAcwAuAHgAKQAgAC8AIABzAHAAYQBjAGUAKQAgADwAIAB3AGkAZAB0AGgAIAB8AHwAIABmAHIAYQBjACgAKABwAG8AcwAuAHoAKwBvAGYAZgBzAC4AegApACAALwAgAHMAcABhAGMAZQApACAAPAAgAHcAaQBkAHQAaAApAAoACQByAGUAdAB1AHIAbgAgAGEAbABwAGgAYQA7AAoAcgBlAHQAdQByAG4AIAAwAC4AMAA7AA==,output:0,fname:Function_node_150,width:465,height:148,input:2,input:0,input:0,input:0,input:2,input_1_label:pos,input_2_label:space,input_3_label:alpha,input_4_label:width,input_5_label:offs|A-5269-XYZ,B-4064-OUT,C-7822-OUT,D-7350-OUT,E-9055-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:5269,x:31760,y:33111,varname:node_5269,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:4064,x:31760,y:33281,ptovrint:False,ptlb:GridSpacing,ptin:_GridSpacing,varname:node_4064,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Color,id:2578,x:32291,y:32584,ptovrint:False,ptlb:HighlightColor,ptin:_HighlightColor,varname:node_2578,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0.1310346,c4:1;n:type:ShaderForge.SFN_Vector4Property,id:254,x:31431,y:32865,ptovrint:False,ptlb:HighlightPos,ptin:_HighlightPos,varname:node_254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Code,id:2855,x:31682,y:32860,varname:node_2855,prsc:2,code:ZgBsAG8AYQB0ADMAIABkAGkAZgAgAD0AIABoAGkALQB3AG8AOwAKAHIAZQB0AHUAcgBuACAAcABvAHcAKABzAHEAcgB0ACgAKABkAGkAZgAuAHgAKgBkAGkAZgAuAHgAKQArACgAZABpAGYALgB6ACoAZABpAGYALgB6ACkAKQAvACgAZABzAHQALwAyAC4AMAApACwAZQBkAGcAZQApADsA,output:0,fname:Function_node_2855,width:527,height:194,input:2,input:2,input:0,input:0,input_1_label:hi,input_2_label:wo,input_3_label:dst,input_4_label:edge|A-254-XYZ,B-5253-XYZ,C-314-OUT,D-102-OUT;n:type:ShaderForge.SFN_Lerp,id:3097,x:32497,y:32734,varname:node_3097,prsc:2|A-2578-RGB,B-1304-RGB,T-1611-OUT;n:type:ShaderForge.SFN_ValueProperty,id:314,x:31431,y:33181,ptovrint:False,ptlb:HighlightDist,ptin:_HighlightDist,varname:node_314,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.4;n:type:ShaderForge.SFN_FragmentPosition,id:5253,x:31431,y:33029,varname:node_5253,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:102,x:31431,y:33261,ptovrint:False,ptlb:HighlightEdge,ptin:_HighlightEdge,varname:node_102,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:30;n:type:ShaderForge.SFN_ValueProperty,id:7350,x:31760,y:33375,ptovrint:False,ptlb:LineWidth,ptin:_LineWidth,varname:node_7350,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Clamp01,id:1611,x:32291,y:32826,varname:node_1611,prsc:2|IN-2855-OUT;n:type:ShaderForge.SFN_Vector4Property,id:9055,x:31760,y:33454,ptovrint:False,ptlb:GridOffset,ptin:_GridOffset,varname:node_9055,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0,v3:0.5,v4:0;n:type:ShaderForge.SFN_Lerp,id:7822,x:32497,y:32910,varname:node_7822,prsc:2|A-2578-A,B-1304-A,T-1611-OUT;proporder:1304-4064-254-2578-314-102-7350-9055;pass:END;sub:END;*/

Shader "Shader Forge/GridShader" {
    Properties {
        _Color ("Color", Color) = (1,0,0,0.447)
        _GridSpacing ("GridSpacing", Float ) = 1
        _HighlightPos ("HighlightPos", Vector) = (0,0,0,0)
        _HighlightColor ("HighlightColor", Color) = (0,1,0.1310346,1)
        _HighlightDist ("HighlightDist", Float ) = 1.4
        _HighlightEdge ("HighlightEdge", Float ) = 30
        _LineWidth ("LineWidth", Float ) = 0.1
        _GridOffset ("GridOffset", Vector) = (0.5,0,0.5,0)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent-10"
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
            float Function_node_150( float3 pos , float space , float alpha , float width , float3 offs ){
            if(frac((pos.x+offs.x) / space) < width || frac((pos.z+offs.z) / space) < width)
            	return alpha;
            return 0.0;
            }
            
            uniform float _GridSpacing;
            uniform float4 _HighlightColor;
            uniform float4 _HighlightPos;
            float Function_node_2855( float3 hi , float3 wo , float dst , float edge ){
            float3 dif = hi-wo;
            return pow(sqrt((dif.x*dif.x)+(dif.z*dif.z))/(dst/2.0),edge);
            }
            
            uniform float _HighlightDist;
            uniform float _HighlightEdge;
            uniform float _LineWidth;
            uniform float4 _GridOffset;
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
                fixed4 finalRGBA = fixed4(finalColor,Function_node_150( i.posWorld.rgb , _GridSpacing , lerp(_HighlightColor.a,_Color.a,node_1611) , _LineWidth , _GridOffset.rgb ));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

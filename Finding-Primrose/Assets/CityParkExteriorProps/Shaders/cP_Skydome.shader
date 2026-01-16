// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:2,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:3554,x:32840,y:32962,varname:node_3554,prsc:2|emission-582-OUT,custl-7118-OUT;n:type:ShaderForge.SFN_Color,id:8306,x:31772,y:32686,ptovrint:False,ptlb:Sky Color,ptin:_SkyColor,varname:node_8306,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.02553246,c2:0.03709318,c3:0.1827586,c4:1;n:type:ShaderForge.SFN_ViewVector,id:2265,x:31161,y:32872,varname:node_2265,prsc:2;n:type:ShaderForge.SFN_Dot,id:7606,x:31418,y:32953,varname:node_7606,prsc:2,dt:1|A-2265-OUT,B-3211-OUT;n:type:ShaderForge.SFN_Vector3,id:3211,x:31161,y:32997,varname:node_3211,prsc:2,v1:0,v2:-1,v3:0;n:type:ShaderForge.SFN_Color,id:3839,x:31772,y:32848,ptovrint:False,ptlb:Horizon Color,ptin:_HorizonColor,varname:_GroundColor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.06617647,c2:0.5468207,c3:1,c4:1;n:type:ShaderForge.SFN_Power,id:4050,x:31772,y:32995,varname:node_4050,prsc:2|VAL-6125-OUT,EXP-7609-OUT;n:type:ShaderForge.SFN_Vector1,id:7609,x:31587,y:33095,varname:node_7609,prsc:2,v1:8;n:type:ShaderForge.SFN_OneMinus,id:6125,x:31587,y:32953,varname:node_6125,prsc:2|IN-7606-OUT;n:type:ShaderForge.SFN_Lerp,id:2737,x:31999,y:32869,cmnt:Sky,varname:node_2737,prsc:2|A-8306-RGB,B-3839-RGB,T-4050-OUT;n:type:ShaderForge.SFN_LightVector,id:3559,x:30723,y:33040,cmnt:Auto-adapts to your directional light,varname:node_3559,prsc:2;n:type:ShaderForge.SFN_Dot,id:1472,x:31082,y:33150,cmnt:Linear falloff to sun angle,varname:node_1472,prsc:2,dt:1|A-8269-OUT,B-8750-OUT;n:type:ShaderForge.SFN_ViewVector,id:8750,x:30895,y:33160,varname:node_8750,prsc:2;n:type:ShaderForge.SFN_Add,id:7568,x:32262,y:33059,cmnt:Sky plus Sun,varname:node_7568,prsc:2|A-2737-OUT,B-5855-OUT;n:type:ShaderForge.SFN_Negate,id:8269,x:30895,y:33040,varname:node_8269,prsc:2|IN-3559-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:3001,x:31383,y:33282,cmnt:Modify radius of falloff,varname:node_3001,prsc:2|IN-1472-OUT,IMIN-1476-OUT,IMAX-1574-OUT,OMIN-9430-OUT,OMAX-6262-OUT;n:type:ShaderForge.SFN_Slider,id:2435,x:30320,y:33466,ptovrint:False,ptlb:Sun Radius B,ptin:_SunRadiusB,varname:node_2435,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:0.1;n:type:ShaderForge.SFN_Slider,id:3144,x:30320,y:33360,ptovrint:False,ptlb:Sun Radius A,ptin:_SunRadiusA,varname:_SunOuterRadius_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;n:type:ShaderForge.SFN_Vector1,id:9430,x:31082,y:33610,varname:node_9430,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6262,x:31082,y:33668,varname:node_6262,prsc:2,v1:0;n:type:ShaderForge.SFN_Clamp01,id:7022,x:31556,y:33282,varname:node_7022,prsc:2|IN-3001-OUT;n:type:ShaderForge.SFN_OneMinus,id:1574,x:31082,y:33464,varname:node_1574,prsc:2|IN-8889-OUT;n:type:ShaderForge.SFN_OneMinus,id:1476,x:31082,y:33315,varname:node_1476,prsc:2|IN-3432-OUT;n:type:ShaderForge.SFN_Multiply,id:8889,x:30893,y:33464,varname:node_8889,prsc:2|A-9367-OUT,B-9367-OUT;n:type:ShaderForge.SFN_Multiply,id:3432,x:30893,y:33315,varname:node_3432,prsc:2|A-7933-OUT,B-7933-OUT;n:type:ShaderForge.SFN_Max,id:9367,x:30681,y:33464,varname:node_9367,prsc:2|A-3144-OUT,B-2435-OUT;n:type:ShaderForge.SFN_Min,id:7933,x:30681,y:33315,varname:node_7933,prsc:2|A-3144-OUT,B-2435-OUT;n:type:ShaderForge.SFN_Power,id:754,x:31772,y:33336,varname:node_754,prsc:2|VAL-7022-OUT,EXP-5929-OUT;n:type:ShaderForge.SFN_Vector1,id:5929,x:31556,y:33412,varname:node_5929,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:5855,x:31957,y:33257,cmnt:Sun,varname:node_5855,prsc:2|A-2359-RGB,B-754-OUT,C-7055-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7055,x:31772,y:33484,ptovrint:False,ptlb:Sun Intensity,ptin:_SunIntensity,varname:node_7055,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_LightColor,id:2359,x:31772,y:33210,cmnt:Get color from directional light,varname:node_2359,prsc:2;n:type:ShaderForge.SFN_Time,id:3983,x:31370,y:33729,varname:node_3983,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:2014,x:31370,y:33991,ptovrint:False,ptlb:Cloud 01 Max Speed V,ptin:_Cloud01MaxSpeedV,varname:node_2014,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:9792,x:31370,y:33891,ptovrint:False,ptlb:Cloud 01 Min Speed V,ptin:_Cloud01MinSpeedV,varname:node_9792,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Slider,id:8338,x:31213,y:34084,ptovrint:False,ptlb:Cloud 01 Offset Speed V,ptin:_Cloud01OffsetSpeedV,varname:node_8338,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:86,x:31620,y:33907,varname:node_86,prsc:2|A-9792-OUT,B-2014-OUT,T-8338-OUT;n:type:ShaderForge.SFN_Multiply,id:6459,x:31832,y:33823,varname:node_6459,prsc:2|A-3983-TSL,B-86-OUT;n:type:ShaderForge.SFN_Panner,id:762,x:32076,y:33762,varname:node_762,prsc:2,spu:0.2,spv:1|UVIN-6239-UVOUT,DIST-6459-OUT;n:type:ShaderForge.SFN_TexCoord,id:6239,x:31832,y:33660,varname:node_6239,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:7291,x:32261,y:33762,ptovrint:False,ptlb:Cloud 01,ptin:_Cloud01,varname:node_7291,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0405b301413de75498b45458ff56cf86,ntxv:0,isnm:False|UVIN-762-UVOUT;n:type:ShaderForge.SFN_Multiply,id:317,x:32470,y:33762,varname:node_317,prsc:2|A-7291-R,B-6254-OUT;n:type:ShaderForge.SFN_Slider,id:6254,x:32104,y:33959,ptovrint:False,ptlb:Cloud 01 Intensity,ptin:_Cloud01Intensity,varname:node_6254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Time,id:7828,x:31364,y:34203,varname:node_7828,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:7042,x:31364,y:34465,ptovrint:False,ptlb:Cloud 02 Max Speed U,ptin:_Cloud02MaxSpeedU,varname:_node_2014_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:5082,x:31364,y:34365,ptovrint:False,ptlb:Cloud 02 Min Speed U,ptin:_Cloud02MinSpeedU,varname:_node_9792_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Slider,id:7694,x:31207,y:34558,ptovrint:False,ptlb:Cloud 02 Offset Speed U,ptin:_Cloud02OffsetSpeedU,varname:_node_8338_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:4779,x:31614,y:34381,varname:node_4779,prsc:2|A-5082-OUT,B-7042-OUT,T-7694-OUT;n:type:ShaderForge.SFN_Multiply,id:2974,x:31826,y:34297,varname:node_2974,prsc:2|A-7828-TSL,B-4779-OUT;n:type:ShaderForge.SFN_Panner,id:9703,x:32070,y:34236,varname:node_9703,prsc:2,spu:0.65,spv:2.5|UVIN-8910-UVOUT,DIST-2974-OUT;n:type:ShaderForge.SFN_TexCoord,id:8910,x:31826,y:34134,varname:node_8910,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:484,x:32255,y:34236,ptovrint:False,ptlb:Cloud 02,ptin:_Cloud02,varname:_node_7291_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9703-UVOUT;n:type:ShaderForge.SFN_Slider,id:4138,x:32098,y:34441,ptovrint:False,ptlb:Cloud 02 Intensity,ptin:_Cloud02Intensity,varname:node_4138,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:996,x:32476,y:34315,varname:node_996,prsc:2|A-484-R,B-4138-OUT;n:type:ShaderForge.SFN_Multiply,id:7118,x:32866,y:34009,varname:node_7118,prsc:2|A-1242-OUT,B-455-OUT,C-5879-OUT,D-5541-RGB;n:type:ShaderForge.SFN_Slider,id:455,x:32492,y:34196,ptovrint:False,ptlb:Cloud Intensity,ptin:_CloudIntensity,varname:node_455,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Blend,id:1242,x:32649,y:34009,varname:node_1242,prsc:2,blmd:18,clmp:True|SRC-317-OUT,DST-996-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5879,x:32649,y:34315,ptovrint:False,ptlb:Cloud Multiplier,ptin:_CloudMultiplier,varname:node_5879,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.2;n:type:ShaderForge.SFN_Color,id:5541,x:32649,y:34417,ptovrint:False,ptlb:Cloud Color,ptin:_CloudColor,varname:node_5541,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Blend,id:582,x:32505,y:33059,varname:node_582,prsc:2,blmd:12,clmp:True|SRC-7568-OUT,DST-7118-OUT;proporder:8306-3839-2435-3144-7055-5541-455-5879-7291-9792-2014-8338-6254-484-5082-7042-7694-4138;pass:END;sub:END;*/

Shader "Shader Forge/cP_Skydome" {
    Properties {
        _SkyColor ("Sky Color", Color) = (0.02553246,0.03709318,0.1827586,1)
        _HorizonColor ("Horizon Color", Color) = (0.06617647,0.5468207,1,1)
        _SunRadiusB ("Sun Radius B", Range(0, 0.1)) = 0.1
        _SunRadiusA ("Sun Radius A", Range(0, 0.1)) = 0
        _SunIntensity ("Sun Intensity", Float ) = 2
        _CloudColor ("Cloud Color", Color) = (1,1,1,1)
        _CloudIntensity ("Cloud Intensity", Range(0, 1)) = 1
        _CloudMultiplier ("Cloud Multiplier", Float ) = 1.2
        _Cloud01 ("Cloud 01", 2D) = "white" {}
        _Cloud01MinSpeedV ("Cloud 01 Min Speed V", Float ) = 0
        _Cloud01MaxSpeedV ("Cloud 01 Max Speed V", Float ) = 0.5
        _Cloud01OffsetSpeedV ("Cloud 01 Offset Speed V", Range(0, 1)) = 1
        _Cloud01Intensity ("Cloud 01 Intensity", Range(0, 1)) = 1
        _Cloud02 ("Cloud 02", 2D) = "white" {}
        _Cloud02MinSpeedU ("Cloud 02 Min Speed U", Float ) = 0
        _Cloud02MaxSpeedU ("Cloud 02 Max Speed U", Float ) = 0.5
        _Cloud02OffsetSpeedU ("Cloud 02 Offset Speed U", Range(0, 1)) = 1
        _Cloud02Intensity ("Cloud 02 Intensity", Range(0, 1)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "PreviewType"="Skybox"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float4 _SkyColor;
            uniform float4 _HorizonColor;
            uniform float _SunRadiusB;
            uniform float _SunRadiusA;
            uniform float _SunIntensity;
            uniform float _Cloud01MaxSpeedV;
            uniform float _Cloud01MinSpeedV;
            uniform float _Cloud01OffsetSpeedV;
            uniform sampler2D _Cloud01; uniform float4 _Cloud01_ST;
            uniform float _Cloud01Intensity;
            uniform float _Cloud02MaxSpeedU;
            uniform float _Cloud02MinSpeedU;
            uniform float _Cloud02OffsetSpeedU;
            uniform sampler2D _Cloud02; uniform float4 _Cloud02_ST;
            uniform float _Cloud02Intensity;
            uniform float _CloudIntensity;
            uniform float _CloudMultiplier;
            uniform float4 _CloudColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
////// Emissive:
                float node_7933 = min(_SunRadiusA,_SunRadiusB);
                float node_1476 = (1.0 - (node_7933*node_7933));
                float node_9367 = max(_SunRadiusA,_SunRadiusB);
                float node_9430 = 1.0;
                float4 node_3983 = _Time + _TimeEditor;
                float2 node_762 = (i.uv0+(node_3983.r*lerp(_Cloud01MinSpeedV,_Cloud01MaxSpeedV,_Cloud01OffsetSpeedV))*float2(0.2,1));
                float4 _Cloud01_var = tex2D(_Cloud01,TRANSFORM_TEX(node_762, _Cloud01));
                float4 node_7828 = _Time + _TimeEditor;
                float2 node_9703 = (i.uv0+(node_7828.r*lerp(_Cloud02MinSpeedU,_Cloud02MaxSpeedU,_Cloud02OffsetSpeedU))*float2(0.65,2.5));
                float4 _Cloud02_var = tex2D(_Cloud02,TRANSFORM_TEX(node_9703, _Cloud02));
                float3 node_7118 = (saturate((0.5 - 2.0*((_Cloud01_var.r*_Cloud01Intensity)-0.5)*((_Cloud02_var.r*_Cloud02Intensity)-0.5)))*_CloudIntensity*_CloudMultiplier*_CloudColor.rgb);
                float3 node_582 = saturate(((lerp(_SkyColor.rgb,_HorizonColor.rgb,pow((1.0 - max(0,dot(viewDirection,float3(0,-1,0)))),8.0))+(_LightColor0.rgb*pow(saturate((node_9430 + ( (max(0,dot((-1*lightDirection),viewDirection)) - node_1476) * (0.0 - node_9430) ) / ((1.0 - (node_9367*node_9367)) - node_1476))),5.0)*_SunIntensity)) > 0.5 ?  (1.0-(1.0-2.0*((lerp(_SkyColor.rgb,_HorizonColor.rgb,pow((1.0 - max(0,dot(viewDirection,float3(0,-1,0)))),8.0))+(_LightColor0.rgb*pow(saturate((node_9430 + ( (max(0,dot((-1*lightDirection),viewDirection)) - node_1476) * (0.0 - node_9430) ) / ((1.0 - (node_9367*node_9367)) - node_1476))),5.0)*_SunIntensity))-0.5))*(1.0-node_7118)) : (2.0*(lerp(_SkyColor.rgb,_HorizonColor.rgb,pow((1.0 - max(0,dot(viewDirection,float3(0,-1,0)))),8.0))+(_LightColor0.rgb*pow(saturate((node_9430 + ( (max(0,dot((-1*lightDirection),viewDirection)) - node_1476) * (0.0 - node_9430) ) / ((1.0 - (node_9367*node_9367)) - node_1476))),5.0)*_SunIntensity))*node_7118)) );
                float3 emissive = node_582;
                float3 finalColor = emissive + node_7118;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

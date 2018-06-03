using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    // returns true if the variant should be stripped.
    public delegate bool VariantStrippingFunc(HDRenderPipelineAsset hdrpAsset, Shader shader, ShaderSnippetData snippet, ShaderCompilerData inputData);

    public class ShaderPreprocessorMaterial
    {
        protected ShaderKeyword m_ShadowMask;
        protected ShaderKeyword m_Transparent;
        protected ShaderKeyword m_DebugDisplay;
        protected ShaderKeyword m_TileLighting;
        protected ShaderKeyword m_ClusterLighting;

        public ShaderPreprocessorMaterial()
        {
            m_Transparent = new ShaderKeyword("_SURFACE_TYPE_TRANSPARENT");
            m_DebugDisplay = new ShaderKeyword("DEBUG_DISPLAY");
            m_TileLighting = new ShaderKeyword("USE_FPTL_LIGHTLIST");
            m_ClusterLighting = new ShaderKeyword("USE_CLUSTERED_LIGHTLIST");
        }

        public virtual void AddStripperFuncs(Dictionary<string, VariantStrippingFunc> stripperFuncs) {}

        // NOTE: All these keyword should be automatically stripped so there's no need to handle them ourselves.
        // LIGHTMAP_ON, DIRLIGHTMAP_COMBINED, DYNAMICLIGHTMAP_ON, LIGHTMAP_SHADOW_MIXING, SHADOWS_SHADOWMASK
        // FOG_LINEAR, FOG_EXP, FOG_EXP2
        // STEREO_INSTANCING_ON, STEREO_MULTIVIEW_ON, STEREO_CUBEMAP_RENDER_ON, UNITY_SINGLE_PASS_STEREO
        // INSTANCING_ON

        // Several pass are common to all shader, let's share code here
        // This remove variant (return true) for:
        // - Scene Selection
        // - Motion vectors
        // - Tile pass for Transparent (not compatible)
        // - 
        protected bool CommonShaderStripper(HDRenderPipelineAsset hdrpAsset, Shader shader, ShaderSnippetData snippet, ShaderCompilerData inputData)
        {
            bool isSceneSelectionPass = snippet.passName == "SceneSelectionPass";
            if (isSceneSelectionPass)
                return true;

            bool isMotionPass = snippet.passName == "Motion Vectors";
            if (!hdrpAsset.renderPipelineSettings.supportMotionVectors && isMotionPass)
                return true;

            //bool isForwardPass = (snippet.passName == "Forward") || (snippet.passName == "ForwardOnly");

            if (inputData.shaderKeywordSet.IsEnabled(m_Transparent))
            {
                // If we are transparent we use cluster lighting and not tile lighting
                if (inputData.shaderKeywordSet.IsEnabled(m_TileLighting))
                    return true;
            }
            else // Opaque
            {
                // Note: we can't assume anything regarding tile/cluster for opaque as multiple view could used different settings adn it depends on MSAA
            }

            // TODO: If static lighting we can remove meta pass, but how to know that?

            // TODO: Expose development build flag.
            //if (developmentBuild && inputData.shaderKeywordSet.IsEnabled(m_DebugDisplay))
            //    return true;

            return false;
        }
    }
}

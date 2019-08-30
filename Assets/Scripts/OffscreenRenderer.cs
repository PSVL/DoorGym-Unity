/*
Copyright 2019 Panasonic Beta, a division of Panasonic Corporation of North America

Redistribution and use of this file (hereafter "Software") in source and 
binary forms, with or without modification, are permitted provided that 
the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors
may be used to endorse or promote products derived from this software
without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
THE POSSIBILITY OF SUCH DAMAGE.
*/

using UnityEngine;
using System.Collections;

public class OffscreenRenderer
{
    // offscreen rendering
    private RenderTexture depthrt;
    private RenderTexture offrt;

    private Texture2D offtex;
    private Texture2D depthoftex;

    public int Width { get; }
    public int Height { get; }

    // Use this for initialization
    public OffscreenRenderer(int _width, int _height)
    {
        Width = _width;
        Height = _height;

        // prepare offscreen rendering
        offtex = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        offrt = new RenderTexture(Width, Height, 24);
        offrt.width = Width;
        offrt.height = Height;
        offrt.Create();

        depthoftex = new Texture2D(Width, Height, TextureFormat.RFloat, false);
        depthrt = new RenderTexture(Width, Height, 32, RenderTextureFormat.Depth);
        depthrt.width = Width;
        depthrt.height = Height;
        depthrt.Create();

    }

    ~OffscreenRenderer()
    {
    //    offrt.Release();
    //    segoffrt.Release();
    }

    public Texture2D RenderColor(Camera thecamera)
    {
        // set to offscreen and render
       // thecamera.targetTexture = offrt;
        thecamera.SetTargetBuffers(offrt.colorBuffer, depthrt.depthBuffer);
        thecamera.depthTextureMode = DepthTextureMode.Depth;
        thecamera.Render();

        // read pixels in regular texure and save
        RenderTexture.active = offrt;
        offtex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        offtex.Apply();

        RenderTexture.active = null;
        thecamera.targetTexture = null;
        return offtex;
    }

    public Texture2D ReadDepth(Camera thecamera)
    {
        RenderTexture.active = depthrt;
        depthoftex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        depthoftex.Apply();
        RenderTexture.active = null;
        return depthoftex;
    }

    public int GetColorBufferSize()
    {
        return 3 * Width * Height;
    }

    public int GetSegmentationBufferSize()
    {
        return 3 * Width * Height;
    }

    public Texture2D RenderSegmentation(Camera thecamera)
    {
        thecamera.GetComponent<SegmentationShader>().Activate();
        RenderColor(thecamera);
        thecamera.GetComponent<SegmentationShader>().Deactivate();
        return offtex;
    }
}
﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Renderer.Buffer;
using Renderer.Core;
using Renderer.Materials;
using Renderer.Mesh;
using Renderer.Rendering;
using Renderer.Shaders;
using Renderer.Textures;
using Renderer.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Renderer
{
  public class Firefly
  {
    private Pipeline pipeline;
    private CanvasHandler canvasHandler;
    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private Color4 clearColor;

    /// <summary>
    /// Window width.
    /// </summary>
    private int resolutionWidth;
    /// <summary>
    /// Window height.
    /// </summary>
    private int resolutionHeight;

    /// <summary>
    /// World units on the x-axis.
    /// </summary>
    private int windowWidth;
    /// <summary>
    /// World units on the y-axis.
    /// </summary>
    private int windowHeight;
    private Matrix4 projectionMatrix;

    private ProjectionType projectionType = ProjectionType.Perspective;

    public ProjectionType ProjectionType
    {
      get
      {
        return projectionType;
      }
      set
      {
        projectionType = value;
        CalculateProjectionMatrix((float)windowWidth / (float)windowHeight);
      }
    }


    public float orthographicSize = 18.0f;

    public float OrthographicSize
    {
      get
      {
        return orthographicSize;
      }
      set
      {
        orthographicSize = value;
        CalculateProjectionMatrix((float)windowWidth / (float)windowHeight);
      }
    }

    private float verticalFieldOfView = (float)System.Math.PI / 2.5f;
    public float VerticalFieldOfView {
      get
      {
        return verticalFieldOfView;
      }
      set
      {
        verticalFieldOfView = value;
        CalculateProjectionMatrix((float)windowWidth / (float)windowHeight);
      }
    }

    public Firefly(int resolutionWidth, int resolutionHeight, int windowWidth, int windowHeight, Material canvasMaterial, bool debug)
    {
      textureManager = new TextureManager();
      shaderManager = new ShaderManager(textureManager.GetFreeTextureUnitCount());
      pipeline = new Pipeline(textureManager, shaderManager);
      canvasHandler = new CanvasHandler(shaderManager, canvasMaterial, resolutionWidth, resolutionHeight, windowWidth, windowHeight);

      clearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
      UpdateWindowDimensions(windowWidth, windowHeight);
      UpdateResolution(resolutionWidth, resolutionHeight);

      if (debug)
      { 
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
      }
    }

    public void Render(WorldObject obj)
    {
      canvasHandler.BindFrameBuffer();
      Clear();
      pipeline.RenderObject(obj);
      canvasHandler.DrawCanvas();
    }

    public void UpdateBackgroundColor(Color4 color)
    {
      clearColor = color;
    }

    /// <summary>
    /// Update the GL viewport.
    /// </summary>
    /// <param name="windowWidth">Window width.</param>
    /// <param name="windowHeight">Window height.</param>
    public void UpdateWindowDimensions(int windowWidth, int windowHeight)
    {
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
      canvasHandler.UpdateWindowDimensions(windowWidth, windowHeight);
    }

    /// <summary>
    /// Update the canvas resolution.
    /// </summary>
    /// <param name="resolutionWidth">Resolution width.</param>
    /// <param name="resolutionWidth">Resolution height.</param>
    public void UpdateResolution(int resolutionWidth, int resolutionHeight)
    {
      this.resolutionWidth = resolutionWidth;
      this.resolutionHeight = resolutionHeight;
      canvasHandler.UpdateResolution(resolutionWidth, resolutionHeight);
      pipeline.UpdateResolution(resolutionWidth, resolutionHeight);
      CalculateProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
    }

    /// <summary>
    /// Clear the screen and set OpenGL states to prepare for rendering.
    /// </summary>
    public void Clear()
    {
      Color4 c = clearColor;
      GL.ClearColor(c.R, c.G, c.B, c.A);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    /// <summary>
    /// Calculate the screen to clip-space matrix.
    /// </summary>
    private void CalculateProjectionMatrix(float aspect)
    {
      if (projectionType == ProjectionType.Perspective)
      {
        projectionMatrix = Projection.CreatePerspectiveMatrix(verticalFieldOfView, aspect, 0.001f, 1000f);
      } else if (projectionType == ProjectionType.Orthographic)
      {
        projectionMatrix = Projection.CreateOrthographicMatrix(orthographicSize, aspect, 0.001f, 1000f);
      }

      pipeline.UpdateProjectionMatrix(projectionMatrix);
    }

    private void DebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
      string messageString = Marshal.PtrToStringAnsi(message, length);
      Console.WriteLine($"{severity} {type} | {messageString}");

      if (type == DebugType.DebugTypeError)
      {
        throw new Exception(messageString);
      }
    }

    /// <summary>
    /// Destroy the batch buffers.
    /// </summary>
    public void Destroy()
    {

    }
  }
}

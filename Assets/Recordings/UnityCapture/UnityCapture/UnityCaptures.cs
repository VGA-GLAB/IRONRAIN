/*
  Unity Capture
  Copyright (c) 2018 Bernhard Schelling

  Feature contributors:
    Brandon J Matthews (low-level interface for custom texture capture)

  Based on UnityCam
  https://github.com/mrayy/UnityCam
  Copyright (c) 2016 MHD Yamen Saraiji

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class UnityCaptures : MonoBehaviour
{
	public enum ECaptureDevice
	{
		CaptureDevice1 = 0,
		CaptureDevice2 = 1,
		CaptureDevice3 = 2,
		CaptureDevice4 = 3,
		CaptureDevice5 = 4,
		CaptureDevice6 = 5,
		CaptureDevice7 = 6,
		CaptureDevice8 = 7,
		CaptureDevice9 = 8,
		CaptureDevice10 = 9
	}

	public enum ECaptureSendResult
	{
		SUCCESS = 0,
		WARNING_FRAMESKIP = 1,
		WARNING_CAPTUREINACTIVE = 2,
		ERROR_UNSUPPORTEDGRAPHICSDEVICE = 100,
		ERROR_PARAMETER = 101,
		ERROR_TOOLARGERESOLUTION = 102,
		ERROR_TEXTUREFORMAT = 103,
		ERROR_READTEXTURE = 104,
		ERROR_INVALIDCAPTUREINSTANCEPTR = 200
	}

	public enum EMirrorMode
	{
		Disabled = 0,
		MirrorHorizontally = 1
	}

	public enum EResizeMode
	{
		Disabled = 0,
		LinearResize = 1
	}

	public enum SelectRenderer
	{
		First = 0,
		Second = 1,
		Third = 2,
		Fourth = 3
	}

	[SerializeField] [Tooltip("Capture device index")]
	public ECaptureDevice CaptureDevice = ECaptureDevice.CaptureDevice1;

	[SerializeField]
	[Tooltip("Scale image if Unity and capture resolution don't match (can introduce frame dropping, not recommended)")]
	public EResizeMode ResizeMode = EResizeMode.Disabled;

	[SerializeField]
	[Tooltip("How many milliseconds to wait for a new frame until sending is considered to be stopped")]
	public int Timeout = 1000;

	[SerializeField] [Tooltip("Mirror captured output image")]
	public EMirrorMode MirrorMode = EMirrorMode.Disabled;

	[SerializeField] [Tooltip("Introduce a frame of latency in favor of frame rate")]
	public bool DoubleBuffering;

	[SerializeField] [Tooltip("Check to enable VSync during capturing")]
	public bool EnableVSync;

	[SerializeField] [Tooltip("Set the desired render target frame rate")]
	public int TargetFrameRate = 60;

	[SerializeField] [Tooltip("Check to disable output of warnings")]
	public bool HideWarnings;

	[SerializeField] private RenderTexture[] _cameraRender;
	[SerializeField] private SelectRenderer _selectRenderer = SelectRenderer.First;
	[SerializeField] private bool _isCapture;

	private Interface CaptureInterface;

	private void Awake()
	{
		QualitySettings.vSyncCount = EnableVSync ? 1 : 0;
		Application.targetFrameRate = TargetFrameRate;

		if (Application.runInBackground == false)
		{
			Debug.LogWarning("Application.runInBackground switched to enabled for capture streaming");
			Application.runInBackground = true;
		}
	}

	private void Start()
	{
		CaptureInterface = new Interface(ECaptureDevice.CaptureDevice1);
	}

	private void Update()
	{
		if (!_isCapture) return;
		if (_cameraRender.Length <= (int)_selectRenderer) return;

		CaptureInterface.SendTexture(_cameraRender[(int)_selectRenderer], Timeout, DoubleBuffering, ResizeMode,
			MirrorMode);
	}

	private void OnDestroy()
	{
		CaptureInterface.Close();
	}

	//動画撮影するかどうか
	public void ChangeIsCapture(bool isCapture) =>_isCapture = isCapture;
//録画するかめら
	public void SelectCapture(SelectRenderer pickRenderer)=> _selectRenderer = pickRenderer;

	public class Interface
	{
		private IntPtr CaptureInstance;

		public Interface(ECaptureDevice CaptureDevice)
		{
			CaptureInstance = CaptureCreateInstance((int)CaptureDevice);
		}

		[DllImport("UnityCapturePlugin")]
		private static extern IntPtr CaptureCreateInstance(int CapNum);

		[DllImport("UnityCapturePlugin")]
		private static extern void CaptureDeleteInstance(IntPtr instance);

		[DllImport("UnityCapturePlugin")]
		private static extern ECaptureSendResult CaptureSendTexture(IntPtr instance, IntPtr nativetexture,
			int Timeout, bool UseDoubleBuffering, EResizeMode ResizeMode, EMirrorMode MirrorMode,
			bool IsLinearColorSpace);

		~Interface()
		{
			Close();
		}

		public void Close()
		{
			if (CaptureInstance != IntPtr.Zero)
			{
				CaptureDeleteInstance(CaptureInstance);
			}

			CaptureInstance = IntPtr.Zero;
		}

		public ECaptureSendResult SendTexture(Texture Source, int Timeout = 1000, bool DoubleBuffering = false,
			EResizeMode ResizeMode = EResizeMode.Disabled, EMirrorMode MirrorMode = EMirrorMode.Disabled)
		{
			if (CaptureInstance == IntPtr.Zero)
			{
				return ECaptureSendResult.ERROR_INVALIDCAPTUREINSTANCEPTR;
			}

			return CaptureSendTexture(CaptureInstance, Source.GetNativeTexturePtr(), Timeout, DoubleBuffering,
				ResizeMode, MirrorMode, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}
	}
}

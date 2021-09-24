# Changelog

## [0.0.10-preview.20] - 2021-03-09

- Fixed an issue with the script updater not working with updating an API. CameraDepth -> CameraDepthTarget.

## [0.0.10-preview.19] - 2021-02-05

- Fix compile error with HDRP 10.2.2 and later (obsolete method use)

## [0.0.10-preview.18] - 2021-02-03

- Added new HDRP shaders to preprocessor list to strip on non-HDRP projects. (fixes shader error on first import)

## [0.0.10-preview.17] - 2021-01-28

- Added support for capturing depth normal and motion vectors channels for all pipelines.
- Fixed a bug where recycled command buffers would not get named for the channel they were being used for.
- Added a ChannelGrab component. Add it to a camera, and select the channel to capture. You can add a component for each channel.

## [0.0.10-preview.16] - 2021-01-07

- CameraGrab and DepthGrab components support an option NameGenerator parameter which can be used to customize the names picked for subsequent captures.
- CaptureCamera supports per channel force flip flags. The capture pathway has been refactored, and should be auto flipped correctly. These flags allow you to override.
- Fixed an issue with IL2CPP where the jpg encoder would crash resizing the compressed array to actual size.
- Fixed an issue reading back single channel textures as R16_UNorm. Values are now properly scaled between 0 and 0xffff inclusive.
- Added many tests for ensuring correct output, orientation, and depth for all pipelines.
- Fixed bug on exiting playmode that would not perform the shutdown properly if the player is paused when exiting playmode.

Known Issues
- Under certain circumstances, having MSAA enabled can cause depth capture to fail.

## [0.0.10-preview.15] - 2020-10-27

- Adding support for cloud rendering build target starting 2019.4 and above

## [0.0.10-preview.14] - 2020-09-28

- Add !USIM_USE_BUILTIN_JPG_ENCODER around JpegEncoder.cs
  This native plugin is only supported on Windows, Mac, Linux, but the builtin jpeg encoder can be enabled
  by setting the build define USIM_USE_BUILTIN_JPG_ENCODER. This change also removes the JpegEncoder.cs
  source that uses the native plugin.

## [0.0.10-preview.13] - 2020-08-27

- Add HDRP_DISABLED shader variant to avoid shader compiler include errors when compiling HDRP shaders for other pipelines.
- Updated com.unity.simulation.editor asmdef, name and added version defines for graphics pipelines.
- Updated CaptureCamera.SelectDepthShaderVariant to select HDRP_ENABLED keyword for that pipeline.

## [0.0.10-preview.12] - 2020-08-26

- Update dependency to com.unity.simulation.core@0.0.10-preview.19

## [0.0.10-preview.11] - 2020-08-20

- Fixed Compile issues with 2020.1.
- Added more test coverage for better stability.
- Fixed FlipY issues with built-in rendering pipeline.
- Fix for BlitCopyDepthHDRP errors while building URP project.
- Support for configurable suffix for logfile names with options to add timestamp and/or seq number.

## [0.0.10-preview.10] - 2020-08-03

- Fixed an issue for images coming out flipped for Metal Graphics API
- Added a public API ShouldFlipY to check if the image readback from backbuffer needs to be flipped.
- Updating core dependency to preview.16

## [0.0.10-preview.9] - 2020-07-21

- Fixed an issue for one captured frame being off while using AsyncGPUReadback request.
- Fixed an issue for RGB capture on Metal Gfx API turning out to be all red while running in OSX Player.
- Fixed an issue for depth capture from a RenderTexture.
- Exposed maxElapsedTime via Simulation Logger constructor.

## [0.0.10-preview.8] - 2020-05-15

- Fixed depth support. Depth can now be captured in all formats, but in order to save it to file, 
  you either need to use a raw format, or one of the supported image formats.
  This will result in depth being 8 bits for PNG/JPG etc.
  When consumed at runtime, full precision of R32_SFloat is supported as a capture format.
  Depth is normalized for the distance between the near and far clipping planes, so scale accordingly.

## [0.0.10-preview.7] - 2020-05-04

- Support for Batch readback for both synchronous and asynchronous readback paths. Improves image readback performance by ~20% on player build with GPU support and by ~5-10% on CPU based build. **
- End to end test coverage on usim
- Use NativeArray to pass for encoding (2020.1 and above)
- Support for 2018.4 restored.

(** : Subject to batch size, image resolution and time taken to render the frame i.e if render time is a bottleneck or the readback time)

## [0.0.10-preview.6] - 2020-03-20

Fix uses of deprecated image encoding API.

## [0.0.10-preview.5] - 2020-03-20

Add package dependency to com.unity.simulation.core@0.0.10-preview.8

## [0.0.10-preview.4] - 2020-03-20

Add package dependency to com.unity.simulation.core@0.0.10-preview.6

## [0.0.10-preview.3] - 2020-03-11

Update third party notification text per legal.

## [0.0.10-preview.2] - 2020-03-10

Add Third Party Notices.md for libturbojpeg use.
Fix malformed comment docs in CaptureRenderTexture.

## [0.0.10-preview.1] - 2020-03-09

This package contains funtionality to perform screen capture and data logging.
First package release based on unity package version 1.0.11.
Update license file.
Validation suite fixes.
Add CI files.

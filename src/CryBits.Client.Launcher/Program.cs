using CryBits.Client.Launcher;

using var launcher = new LauncherApp(args.Contains("--offline"));
launcher.Run();
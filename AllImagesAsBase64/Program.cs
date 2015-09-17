using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AllImagesAsBase64
{
	static class Program
	{
		static void Main(string[] args)
		{
			string imagesPath;
			if (ArgsChecking(args, out imagesPath)) return;

			var task = MainAsync(imagesPath);
			task.Wait();

			Console.ReadLine();
		}

		private static async Task MainAsync(string imagesPath)
		{
			Console.WriteLine("Start base64 files generating...");

			var path = imagesPath;
			var files = await Task.Run(() => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories));

			foreach (var f in files.Where(x => !x.Contains(".b64") && !x.Contains(".swf") && !x.Contains(".psd")))
			{
				Console.WriteLine(f);

				var fileContentBase64 = EncodeToBase64(f);

				if (string.IsNullOrEmpty(fileContentBase64))
				{
					Console.Write(" - not supported format");
				}
				else
				{
					File.WriteAllText(string.Format("{0}.b64", f), fileContentBase64);
				}
			}

			Console.WriteLine(" ... done");
		}

		private static string EncodeToBase64(string filePath)
		{
			var splittedText = filePath.ToLower().Split('.');
			var ext = splittedText[splittedText.Length - 1];
			var image = Image.FromFile(filePath);

			ImageFormat imageFormat;

			switch (ext)
			{
				case "png":
					imageFormat = ImageFormat.Png;
					break;

				case "jpg":
				case "jpeg":
					imageFormat = ImageFormat.Jpeg;
					break;

				case "bmp":
					imageFormat = ImageFormat.Bmp;
					break;

				case "gif":
					imageFormat = ImageFormat.Gif;
					break;

				default:
					imageFormat = null;
					break;
			}

			if (imageFormat == null)
			{
				return string.Empty;
			}

			using (var ms = new MemoryStream())
			{
				image.Save(ms, imageFormat);
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		private static bool ArgsChecking(IList<string> args, out string imagesPath)
		{
			imagesPath = string.Empty;

			if (args.Count == 0)
			{
				Console.WriteLine("Please use this options: ");
				Console.WriteLine("-p (--path) \"Directory path\"");
				return true;
			}

			if (args[0] != "-p" && args[0] != "--path")
			{
				return false;
			}

			if (!Directory.Exists(args[1]))
			{
				Console.WriteLine("Specific path does not exists!");
			}

			imagesPath = args[1];
			return false;
		}
	}
}

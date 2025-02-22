﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Docs.InlineTOCGenerator
{
	/// <summary>
	/// Generates a docfx partial file listing items from a subsection of the table of contents.
	/// </summary>
	class InlineTOCGenerator
	{
		public static void Generate(Item topLevelItem, string sectionName, int itemDepth, string targetFolder)
		{
			var targetFile = Path.Combine(targetFolder, $"{SanitizeFilename(sectionName)}-inline-toc.include");
			using var streamWriter = new StreamWriter(targetFile);

			foreach (var line in GenerateInner(topLevelItem, sectionName, itemDepth))
			{
				streamWriter.WriteLine(line);
				streamWriter.WriteLine();
			}
			;
		}

		private static IEnumerable<string> GenerateInner(Item topLevelItem, string sectionName, int itemDepth)
		{
			var item = FindItem(topLevelItem, sectionName) ?? throw new ArgumentNullException(nameof(sectionName));

			return GenerateHeadersOrLinks(item, itemDepth, headerDepth: 1);
		}

		private static IEnumerable<string> GenerateHeadersOrLinks(Item rootItem, int itemDepth, int headerDepth)
		{
			itemDepth--;
			headerDepth++;

			if (itemDepth > 0)
			{
				if (rootItem.Items?.Length > 0)
				{
					foreach (var item in rootItem.Items)
					{
						if (item.Items?.Length > 0)
						{
							//yield return "";
							var headerMark = "".PadLeft(headerDepth, '#');
							yield return $"{headerMark} {item.Name}";
							//yield return "";
							foreach (var str in GenerateHeadersOrLinks(item, itemDepth, headerDepth))
							{
								yield return str;
							}
						}
					}
				}
			}
			else
			{
				foreach (var link in GenerateLinks(rootItem))
				{
					yield return link;
				}
			}
		}

		private static IEnumerable<string> GenerateLinks(Item rootItem)
		{
			if (rootItem.Items == null)
			{
				yield break;
			}

			foreach (var item in rootItem.Items)
			{
				var link = item.Href ?? item.TopicHref;
				if (link == null)
				{
					continue;
				}
				if (item.Name == null)
				{
					continue;
				}
				yield return $" * [{item.Name}](../{link})";
			}
		}

		private static Item? FindItem(Item rootItem, string itemName)
		{
			if (rootItem.Name == itemName)
			{
				return rootItem;
			}

			if (rootItem.Items != null)
			{
				foreach (var childItem in rootItem.Items)
				{
					if (FindItem(childItem, itemName) is { } match)
					{
						return match;
					}
				}
			}

			return null;
		}

		private static string SanitizeFilename(string originalStr)
		{
			return originalStr.ToLowerInvariant().Replace(' ', '-');
		}
	}
}

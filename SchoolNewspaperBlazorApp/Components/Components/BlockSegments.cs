namespace SchoolNewspaperBlazorApp.Components.Components
{
    public enum BlockSegmentKind
    {
        Text,
        // Jedno zdjęcie wyświetlane osobno.
        Single,
        // Kilka kolejnych zdjęć scalonych w jeden kolaż.
        Collage
    }

    public sealed class BlockSegment<T>
    {
        public BlockSegmentKind Kind { get; set; }
        public List<T> Items { get; } = new();
    }

    public static class BlockSegments
    {
        // Tyle zdjęć mieści jeden kolaż; dłuższy ciąg zdjęć dzieli się na kolejne kolaże.
        public const int MaxCollageSize = 6;

        // Zamienia listę bloków na segmenty do wyświetlenia. Kolejne zdjęcia z włączonym scalaniem
        // tworzą jeden kolaż; tekst albo zdjęcie z wyłączonym scalaniem przerywa ciąg.
        // Ta sama logika służy edytorowi, podglądowi i stronie artykułu, więc wszędzie wygląda to tak samo.
        public static List<BlockSegment<T>> Build<T>(IEnumerable<T> blocks, Func<T, bool> isImage, Func<T, bool> inCollage)
        {
            var result = new List<BlockSegment<T>>();
            BlockSegment<T>? run = null;

            foreach (var block in blocks)
            {
                if (!isImage(block))
                {
                    run = null;
                    var text = new BlockSegment<T> { Kind = BlockSegmentKind.Text };
                    text.Items.Add(block);
                    result.Add(text);
                    continue;
                }

                if (!inCollage(block))
                {
                    run = null;
                    var single = new BlockSegment<T> { Kind = BlockSegmentKind.Single };
                    single.Items.Add(block);
                    result.Add(single);
                    continue;
                }

                if (run == null || run.Items.Count >= MaxCollageSize)
                {
                    run = new BlockSegment<T> { Kind = BlockSegmentKind.Collage };
                    result.Add(run);
                }
                run.Items.Add(block);
            }

            // Kolaż z jednego zdjęcia to po prostu zwykłe zdjęcie.
            foreach (var segment in result)
            {
                if (segment.Kind == BlockSegmentKind.Collage && segment.Items.Count == 1)
                {
                    segment.Kind = BlockSegmentKind.Single;
                }
            }
            return result;
        }
    }

    // Blok treści gotowy do wyświetlenia (strona artykułu i podgląd w edytorze).
    public sealed class BlockViewItem
    {
        public Guid Key { get; init; } = Guid.NewGuid();
        public bool IsImage { get; init; }
        public string? Text { get; init; }
        public string? ImageUrl { get; init; }
        public bool InCollage { get; init; } = true;
    }
}

using Lyricify.Lyrics.Providers.Web.Musixmatch;

namespace Lyricify.Lyrics.Searchers
{
    public class MusixmatchSearchResult : SearchResult
    {
        public override ISearcher Searcher => new MusixmatchSearcher();

        public MusixmatchSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id, string isrc, string vanityId) : base(title, artists, album, albumArtists, durationMs)
        {
            Id = id;
            Isrc = isrc;
            VanityId = vanityId;
        }

        public MusixmatchSearchResult(GetTrackResponse.Track track) : this(
            track.TrackName,
            track.ArtistName.Split(new string[] { " feat. ", " & " }, StringSplitOptions.RemoveEmptyEntries),
            track.AlbumName,
            null,
            track.TrackLength * 1000,
            track.TrackId,
            track.TrackIsrc,
            track.CommontrackVanityId
            )
        { }

        public int Id { get; }

        public string Isrc { get; }

        public string VanityId { get; set; }
    }
}

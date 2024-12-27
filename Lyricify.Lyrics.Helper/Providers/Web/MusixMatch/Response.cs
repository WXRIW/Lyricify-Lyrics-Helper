using System.Text.Json.Serialization;

#nullable disable
namespace Lyricify.Lyrics.Providers.Web.Musixmatch
{
    public class GetTokenResponse
    {
        [JsonPropertyName("message")]
        public GetTokenResponse_MessageContent Message { get; set; }

        public class GetTokenResponse_MessageContent
        {
            [JsonPropertyName("header")]
            public GetTokenResponse_MessageContent_Header Header { get; set; }

            [JsonPropertyName("body")]
            public GetTokenResponse_MessageContent_Body Body { get; set; }
        }

        public class GetTokenResponse_MessageContent_Header
        {
            [JsonPropertyName("status_code")]
            public int StatusCode { get; set; }

            [JsonPropertyName("execute_time")]
            public double ExecuteTime { get; set; }

            [JsonPropertyName("pid")]
            public int Pid { get; set; }

            [JsonPropertyName("surrogate_key_list")]
            public object[] SurrogateKeyList { get; set; }

            [JsonPropertyName("hint")]
            public string Hint { get; set; }
        }

        public class GetTokenResponse_MessageContent_Body
        {
            [JsonPropertyName("user_token")]
            public string UserToken { get; set; }

            [JsonPropertyName("app_config")]
            public AppConfig AppConfig { get; set; }

            [JsonPropertyName("location")]
            public Location Location { get; set; }
        }

        public class AppConfig
        {
            [JsonPropertyName("trial")]
            public bool Trial { get; set; }

            [JsonPropertyName("mobilePopOverMaximum")]
            public int MobilePopOverMaximum { get; set; }

            [JsonPropertyName("mobilePopOverMinTimes")]
            public int MobilePopOverMinTimes { get; set; }

            [JsonPropertyName("mobilePopOverMaxTimes")]
            public int MobilePopOverMaxTimes { get; set; }

            [JsonPropertyName("isRoviCopyEnabled")]
            public bool IsRoviCopyEnabled { get; set; }

            [JsonPropertyName("isGettyCopyEnabled")]
            public bool IsGettyCopyEnabled { get; set; }

            [JsonPropertyName("searchMaxResults")]
            public int SearchMaxResults { get; set; }

            [JsonPropertyName("fbShareUrlSpotify")]
            public bool FbShareUrlSpotify { get; set; }

            [JsonPropertyName("twShareUrlSpotify")]
            public bool TwShareUrlSpotify { get; set; }

            [JsonPropertyName("fbPostTimeline")]
            public bool FbPostTimeline { get; set; }

            [JsonPropertyName("fbOpenGraph")]
            public bool FbOpenGraph { get; set; }

            [JsonPropertyName("subtitlesMaxDeviation")]
            public int SubtitlesMaxDeviation { get; set; }

            [JsonPropertyName("localeDefaultLang")]
            public string LocaleDefaultLang { get; set; }

            [JsonPropertyName("missionEnable")]
            public bool MissionEnable { get; set; }

            [JsonPropertyName("content_team")]
            public bool ContentTeam { get; set; }

            [JsonPropertyName("missions")]
            public Missions Missions { get; set; }

            [JsonPropertyName("mission_manager_categories")]
            public MissionManagerCategories MissionManagerCategories { get; set; }

            [JsonPropertyName("appstore_products")]
            public object[] AppstoreProducts { get; set; }

            [JsonPropertyName("tracking_list")]
            public TrackingList[] TrackingList { get; set; }

            [JsonPropertyName("spotifyCountries")]
            public string[] SpotifyCountries { get; set; }

            [JsonPropertyName("service_list")]
            public ServiceList[] ServiceList { get; set; }

            [JsonPropertyName("show_amazon_music")]
            public bool ShowAmazonMusic { get; set; }

            [JsonPropertyName("isSentryEnabled")]
            public bool IsSentryEnabled { get; set; }

            [JsonPropertyName("languages")]
            public string[] Languages { get; set; }

            [JsonPropertyName("last_updated")]
            public DateTime LastUpdated { get; set; }

            [JsonPropertyName("cluster")]
            public string Cluster { get; set; }

            [JsonPropertyName("event_map")]
            public EventMap[] EventMap { get; set; }

            [JsonPropertyName("mission_report_types")]
            public string[] MissionReportTypes { get; set; }

            [JsonPropertyName("can_show_performers_tag")]
            public bool CanShowPerformersTag { get; set; }

            [JsonPropertyName("smart_translations")]
            public SmartTranslations SmartTranslations { get; set; }

            [JsonPropertyName("audio_upload")]
            public bool AudioUpload { get; set; }
        }

        public class Missions
        {
            [JsonPropertyName("enabled")]
            public bool Enabled { get; set; }

            [JsonPropertyName("community")]
            public string[] Community { get; set; }
        }

        public class MissionManagerCategories
        {
            [JsonPropertyName("taskTypes")]
            public TaskType[] TaskTypes { get; set; }

            [JsonPropertyName("audioSources")]
            public AudioSource[] AudioSources { get; set; }

            [JsonPropertyName("userGroups")]
            public UserGroup[] UserGroups { get; set; }

            [JsonPropertyName("rewards")]
            public Reward[] Rewards { get; set; }
        }

        public class TaskType
        {
            [JsonPropertyName("value")]
            public string Value { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }
        }

        public class AudioSource
        {
            [JsonPropertyName("value")]
            public string Value { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }
        }

        public class UserGroup
        {
            [JsonPropertyName("value")]
            public string Value { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }
        }

        public class Reward
        {
            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("value")]
            public string Value { get; set; }
        }

        public class TrackingList
        {
            [JsonPropertyName("tracking")]
            public Tracking Tracking { get; set; }
        }

        public class Tracking
        {
            [JsonPropertyName("context")]
            public string Context { get; set; }

            [JsonPropertyName("delay")]
            public int Delay { get; set; }

            [JsonPropertyName("track_cache_ttl")]
            public int TrackCacheTtl { get; set; }
        }

        public class ServiceList
        {
            [JsonPropertyName("service")]
            public Service Service { get; set; }
        }

        public class Service
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("display_name")]
            public string DisplayName { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("oauth_api")]
            public bool OauthApi { get; set; }

            [JsonPropertyName("oauth_web_signin")]
            public OauthWebSignin OauthWebSignin { get; set; }

            [JsonPropertyName("streaming")]
            public bool Streaming { get; set; }

            [JsonPropertyName("playlist")]
            public bool Playlist { get; set; }

            [JsonPropertyName("locker")]
            public bool Locker { get; set; }

            [JsonPropertyName("deeplink")]
            public bool Deeplink { get; set; }
        }

        public class OauthWebSignin
        {
            [JsonPropertyName("enabled")]
            public bool Enabled { get; set; }

            [JsonPropertyName("user_prefix")]
            public string UserPrefix { get; set; }
        }

        public class EventMap
        {
            [JsonPropertyName("regex")]
            public string Regex { get; set; }

            [JsonPropertyName("enabled")]
            public bool Enabled { get; set; }

            [JsonPropertyName("piggyback")]
            public Piggyback Piggyback { get; set; }
        }

        public class Piggyback
        {
            [JsonPropertyName("server_weight")]
            public int ServerWeight { get; set; }
        }

        public class SmartTranslations
        {
            [JsonPropertyName("enabled")]
            public bool Enabled { get; set; }

            [JsonPropertyName("threshold")]
            public int Threshold { get; set; }
        }

        public class Location
        {
            [JsonPropertyName("GEOIP_CITY_COUNTRY_CODE")]
            public string GeoIPCityCountryCode { get; set; }

            [JsonPropertyName("GEOIP_CITY_COUNTRY_CODE3")]
            public string GeoIPCityCountryCode3 { get; set; }

            [JsonPropertyName("GEOIP_CITY_COUNTRY_NAME")]
            public string GeoIPCityCountryName { get; set; }

            [JsonPropertyName("GEOIP_CITY")]
            public string GeoIPCity { get; set; }

            [JsonPropertyName("GEOIP_CITY_CONTINENT_CODE")]
            public string GeoIPCityContinentCode { get; set; }

            [JsonPropertyName("GEOIP_LATITUDE")]
            public double GeoIPLatitude { get; set; }

            [JsonPropertyName("GEOIP_LONGITUDE")]
            public double GeoIPLongitude { get; set; }

            [JsonPropertyName("GEOIP_AS_ORG")]
            public string GeoIPAsOrg { get; set; }

            [JsonPropertyName("GEOIP_ORG")]
            public string GeoIPOrg { get; set; }

            [JsonPropertyName("GEOIP_ISP")]
            public string GeoIPIsp { get; set; }

            [JsonPropertyName("GEOIP_NET_NAME")]
            public string GeoIPNetName { get; set; }

            [JsonPropertyName("BADIP_TAGS")]
            public object[] BadipTags { get; set; }
        }
    }

    public class GetTrackResponse
    {
        [JsonPropertyName("message")]
        public GetTrackResponse_MessageContent Message { get; set; }

        public class GetTrackResponse_MessageContent
        {
            [JsonPropertyName("header")]
            public GetTrackResponse_MessageContent_Header Header { get; set; }

            [JsonPropertyName("body")]
            public GetTrackResponse_MessageContent_Body Body { get; set; }
        }

        public class GetTrackResponse_MessageContent_Header
        {
            [JsonPropertyName("status_code")]
            public int StatusCode { get; set; }

            [JsonPropertyName("execute_time")]
            public double ExecuteTime { get; set; }

            [JsonPropertyName("confidence")]
            public int Confidence { get; set; }

            [JsonPropertyName("mode")]
            public string Mode { get; set; }

            [JsonPropertyName("cached")]
            public int Cached { get; set; }

            [JsonPropertyName("hint")]
            public string Hint { get; set; }
        }

        public class GetTrackResponse_MessageContent_Body
        {
            [JsonPropertyName("track")]
            public Track Track { get; set; }
        }

        public class Track
        {
            [JsonPropertyName("track_id")]
            public int TrackId { get; set; }

            [JsonPropertyName("track_mbid")]
            public string TrackMbid { get; set; }

            [JsonPropertyName("track_isrc")]
            public string TrackIsrc { get; set; }

            [JsonPropertyName("commontrack_isrcs")]
            public List<List<string>> CommontrackIsrcs { get; set; }

            [JsonPropertyName("track_spotify_id")]
            public string TrackSpotifyId { get; set; }

            [JsonPropertyName("commontrack_spotify_ids")]
            public List<string> CommontrackSpotifyIds { get; set; }

            [JsonPropertyName("track_soundcloud_id")]
            public int TrackSoundcloudId { get; set; }

            [JsonPropertyName("track_xboxmusic_id")]
            public string TrackXboxmusicId { get; set; }

            [JsonPropertyName("track_name")]
            public string TrackName { get; set; }

            [JsonPropertyName("track_name_translation_list")]
            public List<TrackNameTranslationList> TrackNameTranslationList { get; set; }

            [JsonPropertyName("track_rating")]
            public int TrackRating { get; set; }

            [JsonPropertyName("track_length")]
            public int TrackLength { get; set; }

            [JsonPropertyName("commontrack_id")]
            public int CommontrackId { get; set; }

            [JsonPropertyName("instrumental")]
            public int Instrumental { get; set; }

            [JsonPropertyName("explicit")]
            public int Explicit { get; set; }

            [JsonPropertyName("has_lyrics")]
            public int HasLyrics { get; set; }

            [JsonPropertyName("has_lyrics_crowd")]
            public int HasLyricsCrowd { get; set; }

            [JsonPropertyName("has_subtitles")]
            public int HasSubtitles { get; set; }

            [JsonPropertyName("has_richsync")]
            public int HasRichsync { get; set; }

            [JsonPropertyName("has_track_structure")]
            public int HasTrackStructure { get; set; }

            [JsonPropertyName("num_favourite")]
            public int NumFavourite { get; set; }

            [JsonPropertyName("lyrics_id")]
            public int LyricsId { get; set; }

            [JsonPropertyName("subtitle_id")]
            public int SubtitleId { get; set; }

            [JsonPropertyName("album_id")]
            public int AlbumId { get; set; }

            [JsonPropertyName("album_name")]
            public string AlbumName { get; set; }

            [JsonPropertyName("album_vanity_id")]
            public string AlbumVanityId { get; set; }

            [JsonPropertyName("artist_id")]
            public int ArtistId { get; set; }

            [JsonPropertyName("artist_mbid")]
            public string ArtistMbid { get; set; }

            [JsonPropertyName("artist_name")]
            public string ArtistName { get; set; }

            [JsonPropertyName("album_coverart_100x100")]
            public string AlbumCoverart100x100 { get; set; }

            [JsonPropertyName("album_coverart_350x350")]
            public string AlbumCoverart350x350 { get; set; }

            [JsonPropertyName("album_coverart_500x500")]
            public string AlbumCoverart500x500 { get; set; }

            [JsonPropertyName("album_coverart_800x800")]
            public string AlbumCoverart800x800 { get; set; }

            [JsonPropertyName("track_share_url")]
            public string TrackShareUrl { get; set; }

            [JsonPropertyName("track_edit_url")]
            public string TrackEditUrl { get; set; }

            [JsonPropertyName("commontrack_vanity_id")]
            public string CommontrackVanityId { get; set; }

            [JsonPropertyName("restricted")]
            public int Restricted { get; set; }

            [JsonPropertyName("first_release_date")]
            public DateTime FirstReleaseDate { get; set; }

            [JsonPropertyName("updated_time")]
            public DateTime UpdatedTime { get; set; }

            [JsonPropertyName("primary_genres")]
            public PrimaryGenres PrimaryGenres { get; set; }

            [JsonPropertyName("secondary_genres")]
            public SecondaryGenres SecondaryGenres { get; set; }
        }

        public class TrackNameTranslationList
        {
            [JsonPropertyName("track_name_translation")]
            public TrackNameTranslation TrackNameTranslation { get; set; }
        }

        public class TrackNameTranslation
        {
            [JsonPropertyName("language")]
            public string Language { get; set; }

            [JsonPropertyName("translation")]
            public string Translation { get; set; }
        }

        public class PrimaryGenres
        {
            [JsonPropertyName("music_genre_list")]
            public List<MusicGenreList> MusicGenreList { get; set; }
        }

        public class MusicGenreList
        {
            [JsonPropertyName("music_genre")]
            public MusicGenre MusicGenre { get; set; }
        }

        public class MusicGenre
        {
            [JsonPropertyName("music_genre_id")]
            public int MusicGenreId { get; set; }

            [JsonPropertyName("music_genre_parent_id")]
            public int MusicGenreParentId { get; set; }

            [JsonPropertyName("music_genre_name")]
            public string MusicGenreName { get; set; }

            [JsonPropertyName("music_genre_name_extended")]
            public string MusicGenreNameExtended { get; set; }

            [JsonPropertyName("music_genre_vanity")]
            public string MusicGenreVanity { get; set; }
        }

        public class SecondaryGenres
        {
            [JsonPropertyName("music_genre_list")]
            public List<MusicGenreList> MusicGenreList { get; set; }
        }
    }

    public class GetTranslationsResponse
    {
        [JsonPropertyName("message")]
        public GetTranslationsResponse_MessageContent Message { get; set; }

        public class GetTranslationsResponse_MessageContent
        {
            [JsonPropertyName("header")]
            public GetTranslationsResponse_MessageContent_Header Header { get; set; }

            [JsonPropertyName("body")]
            public GetTranslationsResponse_MessageContent_Body Body { get; set; }
        }

        public class GetTranslationsResponse_MessageContent_Header
        {
            [JsonPropertyName("status_code")]
            public int StatusCode { get; set; }

            [JsonPropertyName("execute_time")]
            public double ExecuteTime { get; set; }

            [JsonPropertyName("hint")]
            public string Hint { get; set; }
        }

        public class GetTranslationsResponse_MessageContent_Body
        {
            [JsonPropertyName("translations_list")]
            public TranslationsListItem[] TranslationsList { get; set; }
        }

        public class TranslationsListItem
        {
            [JsonPropertyName("translation")]
            public Translation Translation { get; set; }
        }

        public class Translation
        {
            [JsonPropertyName("type_id")]
            public string TypeId { get; set; }

            [JsonPropertyName("artist_id")]
            public int ArtistId { get; set; }

            [JsonPropertyName("language_from")]
            public string LanguageFrom { get; set; }

            [JsonPropertyName("user_id")]
            public string UserId { get; set; }

            [JsonPropertyName("app_id")]
            public string AppId { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("snippet")]
            public string Snippet { get; set; }

            [JsonPropertyName("position")]
            public int Position { get; set; }

            [JsonPropertyName("selected_language")]
            public string SelectedLanguage { get; set; }

            [JsonPropertyName("index")]
            public int Index { get; set; }

            [JsonPropertyName("wantkey")]
            public bool Wantkey { get; set; }

            [JsonPropertyName("create_timestamp")]
            public int CreateTimestamp { get; set; }

            [JsonPropertyName("language")]
            public string Language { get; set; }

            [JsonPropertyName("type_id_weight")]
            public int TypeIdWeight { get; set; }

            [JsonPropertyName("last_updated")]
            public DateTime LastUpdated { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("matched_line")]
            public string MatchedLine { get; set; }

            [JsonPropertyName("subtitle_matched_line")]
            public string SubtitleMatchedLine { get; set; }

            [JsonPropertyName("confidence")]
            public double Confidence { get; set; }

            [JsonPropertyName("user_score")]
            public int UserScore { get; set; }

            [JsonPropertyName("published_status_macro")]
            public object PublishedStatusMacro { get; set; }

            [JsonPropertyName("image_id")]
            public int ImageId { get; set; }

            [JsonPropertyName("video_id")]
            public int VideoId { get; set; }

            [JsonPropertyName("lyrics_id")]
            public int LyricsId { get; set; }

            [JsonPropertyName("subtitle_id")]
            public int SubtitleId { get; set; }

            [JsonPropertyName("created_date")]
            public DateTime CreatedDate { get; set; }

            [JsonPropertyName("commontrack_id")]
            public int CommontrackId { get; set; }

            [JsonPropertyName("is_expired")]
            public int IsExpired { get; set; }

            [JsonPropertyName("group_key")]
            public string GroupKey { get; set; }

            [JsonPropertyName("can_delete")]
            public int CanDelete { get; set; }

            [JsonPropertyName("is_mine")]
            public int IsMine { get; set; }

            [JsonPropertyName("can_approve")]
            public int CanApprove { get; set; }

            [JsonPropertyName("user")]
            public User User { get; set; }

            [JsonPropertyName("approver")]
            public object Approver { get; set; }

            [JsonPropertyName("can_translate")]
            public int CanTranslate { get; set; }
        }

        public class User
        {
            [JsonPropertyName("uaid")]
            public string Uaid { get; set; }

            [JsonPropertyName("is_mine")]
            public int IsMine { get; set; }

            [JsonPropertyName("user_name")]
            public string UserName { get; set; }

            [JsonPropertyName("user_profile_photo")]
            public string UserProfilePhoto { get; set; }

            [JsonPropertyName("has_private_profile")]
            public int HasPrivateProfile { get; set; }

            [JsonPropertyName("has_informative_profile_page")]
            public int HasInformativeProfilePage { get; set; }

            [JsonPropertyName("has_distributor_profile_page")]
            public int HasDistributorProfilePage { get; set; }

            [JsonPropertyName("score")]
            public int Score { get; set; }

            [JsonPropertyName("position")]
            public int Position { get; set; }

            [JsonPropertyName("level")]
            public string Level { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("rank_level")]
            public int RankLevel { get; set; }

            [JsonPropertyName("rank_name")]
            public string RankName { get; set; }

            [JsonPropertyName("rank_color")]
            public string RankColor { get; set; }

            [JsonPropertyName("rank_colors")]
            public RankColors RankColors { get; set; }

            [JsonPropertyName("rank_image_url")]
            public string RankImageUrl { get; set; }

            [JsonPropertyName("weekly_score")]
            public int WeeklyScore { get; set; }

            [JsonPropertyName("points_to_next_level")]
            public int PointsToNextLevel { get; set; }

            [JsonPropertyName("ratio_to_next_level")]
            public double RatioToNextLevel { get; set; }

            [JsonPropertyName("next_rank_name")]
            public string NextRankName { get; set; }

            [JsonPropertyName("ratio_to_next_rank")]
            public double RatioToNextRank { get; set; }

            [JsonPropertyName("next_rank_color")]
            public string NextRankColor { get; set; }

            [JsonPropertyName("next_rank_colors")]
            public RankColors NextRankColors { get; set; }

            [JsonPropertyName("next_rank_image_url")]
            public string NextRankImageUrl { get; set; }

            [JsonPropertyName("counters")]
            public Counters Counters { get; set; }

            [JsonPropertyName("moderator_eligible")]
            public bool ModeratorEligible { get; set; }

            [JsonPropertyName("artist_manager")]
            public int ArtistManager { get; set; }

            [JsonPropertyName("academy_completed")]
            public bool AcademyCompleted { get; set; }

            [JsonPropertyName("academy_completed_date")]
            public DateTime AcademyCompletedDate { get; set; }
        }

        public class RankColors
        {
            [JsonPropertyName("rank_color_10")]
            public string RankColor10 { get; set; }

            [JsonPropertyName("rank_color_50")]
            public string RankColor50 { get; set; }

            [JsonPropertyName("rank_color_100")]
            public string RankColor100 { get; set; }

            [JsonPropertyName("rank_color_200")]
            public string RankColor200 { get; set; }
        }

        public class Counters
        {
            [JsonPropertyName("lyrics_favourite_added")]
            public int LyricsFavouriteAdded { get; set; }

            [JsonPropertyName("lyrics_subtitle_added")]
            public int LyricsSubtitleAdded { get; set; }

            [JsonPropertyName("lyrics_generic_ko")]
            public int LyricsGenericKo { get; set; }

            [JsonPropertyName("lyrics_missing")]
            public int LyricsMissing { get; set; }

            [JsonPropertyName("lyrics_changed")]
            public int LyricsChanged { get; set; }

            [JsonPropertyName("lyrics_ok")]
            public int LyricsOk { get; set; }

            [JsonPropertyName("lyrics_to_add")]
            public int LyricsToAdd { get; set; }

            [JsonPropertyName("lyrics_ko")]
            public int LyricsKo { get; set; }

            [JsonPropertyName("lyrics_music_id")]
            public int LyricsMusicId { get; set; }

            [JsonPropertyName("vote_bonuses")]
            public int VoteBonuses { get; set; }

            [JsonPropertyName("track_translation")]
            public int TrackTranslation { get; set; }

            [JsonPropertyName("vote_maluses")]
            public int VoteMaluses { get; set; }

            [JsonPropertyName("lyrics_ai_incorrect_text_no")]
            public int LyricsAiIncorrectTextNo { get; set; }

            [JsonPropertyName("lyrics_ai_completely_wrong_skip")]
            public int LyricsAiCompletelyWrongSkip { get; set; }

            [JsonPropertyName("lyrics_ai_incorrect_text_yes")]
            public int LyricsAiIncorrectTextYes { get; set; }

            [JsonPropertyName("lyrics_ai_phrases_not_related_yes")]
            public int LyricsAiPhrasesNotRelatedYes { get; set; }

            [JsonPropertyName("lyrics_implicitly_ok")]
            public int LyricsImplicitlyOk { get; set; }

            [JsonPropertyName("lyrics_report_completely_wrong")]
            public int LyricsReportCompletelyWrong { get; set; }

            [JsonPropertyName("lyrics_richsync_added")]
            public int LyricsRichsyncAdded { get; set; }

            [JsonPropertyName("track_influencer_bonus_moderator_vote")]
            public int TrackInfluencerBonusModeratorVote { get; set; }

            [JsonPropertyName("lyrics_ai_mood_analysis_v3_value")]
            public int LyricsAiMoodAnalysisV3Value { get; set; }

            [JsonPropertyName("lyrics_ai_ugc_language")]
            public int LyricsAiUgcLanguage { get; set; }

            [JsonPropertyName("lyrics_ranking_change")]
            public int LyricsRankingChange { get; set; }

            [JsonPropertyName("track_structure")]
            public int TrackStructure { get; set; }

            [JsonPropertyName("track_complete_metadata")]
            public int TrackCompleteMetadata { get; set; }

            [JsonPropertyName("evaluation_academy_test")]
            public int EvaluationAcademyTest { get; set; }

            [JsonPropertyName("lyrics_report_contain_mistakes")]
            public int LyricsReportContainMistakes { get; set; }
        }
    }
}

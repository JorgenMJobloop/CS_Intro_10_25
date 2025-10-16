public static class ItemMapper
{
    public static ItemDto ToDto(IRentable item) => item switch
    {
        Book book => new ItemDto
        {
            Id = book.Id,
            Title = book.Title,
            _MediaType = book.Type,
            BaseDailyRate = book.BaseDailyRate,
            Author = book.Author,
            Isbn = book.GetType().GetProperty("Isbn")?.GetValue(book) as string,
            IsRented = book.IsRented
        },

        Dvd dvd => new ItemDto
        {
            Id = dvd.Id,
            Title = dvd.Title,
            _MediaType = dvd.Type,
            BaseDailyRate = dvd.BaseDailyRate,
            Runtime = dvd.Runtime,
            Region = dvd.RegionCode,
            ParentalGuidanceRating = dvd.ParentalGuidance,
            IsRented = dvd.IsRented
        },
        Vhs vhs => new ItemDto
        {
            Id = vhs.Id,
            Condition = vhs.Condition,
            ParentalGuidanceRating = vhs.ParentalGuidance
        },
        VideoGames videoGames => new ItemDto
        {
            Id = videoGames.Id,
            ParentalGuidanceRating = videoGames.ParentalGuidance,
            Platform = videoGames.Platform
        },
        _ => throw new NotSupportedException("Unknown type of media was added as input!")
    };

    public static IRentable FromDto(ItemDto dto) => dto._MediaType switch
    {
        MediaType.Book => new Book(dto.Id, dto.Title, dto.Author ?? "Unknown author", dto.Isbn ?? null),
        MediaType.DVD => new Dvd(dto.Id, dto.Title, dto.Region, dto.Runtime, dto.ParentalGuidanceRating),
        MediaType.VHS => new Vhs(dto.Id, dto.Title, dto.Condition ?? "Unknown condition", dto.ParentalGuidanceRating),
        MediaType.VIDEO_GAMES => new VideoGames(dto.Id, dto.Title, dto.Platform, dto.ParentalGuidanceRating),
        _ => throw new NotSupportedException("Unknown type of media added as input!")
    };
}
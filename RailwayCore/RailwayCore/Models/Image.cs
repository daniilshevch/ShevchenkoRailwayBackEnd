using RailwayCore.Models;

public enum ImageType
{
    Profile,
    Station
};
public class Image
{
    public int Id { get; set; }
    public string File_Name { get; set; } = null!;
    public ImageType Type { get; set; }
    public byte[] Image_Data { get; set; } = null!;
    public int? User_Profile_Id { get; set; }
    public UserProfile? User_Profile { get; set; }
}
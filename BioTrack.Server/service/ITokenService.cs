namespace BioTrack.Server.service
{
    public interface ITokenService
    {

        string CreateToken(string username);
    }
}

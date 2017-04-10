namespace Goobeer.Security
{
    /// <summary>
    /// 对称加密结果 数据
    /// </summary>
    public class SymmetricEncryptedData
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] Data { get; set; }
    }
}

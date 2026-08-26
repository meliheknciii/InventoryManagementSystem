using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace InventoryManagement.Tests.Fakes
{
    // Controller'lar TempData kullaniyor (basari mesajlari icin).
    // Test ortaminda gercek bir provider olmadigi icin bos bir tane yaziyoruz.
    public class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(Microsoft.AspNetCore.Http.HttpContext context)
        {
            return new Dictionary<string, object>();
        }

        public void SaveTempData(Microsoft.AspNetCore.Http.HttpContext context, IDictionary<string, object> values)
        {
            // hicbir sey yapmiyoruz, test icin yeterli
        }
    }
}

using Microsoft.AspNetCore.Mvc;

public class BreadcrumbViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string currentPage)
    {
        var breadcrumbItems = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Text = "首頁", Url = "/" },
            new BreadcrumbItem { Text = currentPage, Url = "#", IsActive = true }
        };

        return View(breadcrumbItems);
    }
}

public class BreadcrumbItem
{
    public string Text { get; set; }
    public string Url { get; set; }
    public bool IsActive { get; set; }
}
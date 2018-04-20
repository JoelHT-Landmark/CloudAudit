namespace todo.Controllers
{
    using System.Configuration;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using CloudAudit.Client;
    using Models;
    using todo.ViewModels;

    public class ItemController : Controller
    {
        private readonly IAuditClient auditClient;
        private readonly IAuditReadClient auditReadClient;

        public ItemController()
        {
            this.auditClient = new AuditHttpClient(ConfigurationManager.AppSettings["Audit.ServiceBase"]);
            ////this.auditClient = new AuditServiceBusClient(ConfigurationManager.AppSettings["Audit.ServiceBus"]);

            this.auditReadClient = new AuditHttpClient(ConfigurationManager.AppSettings["Audit.ServiceBase"]);
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);

            await this.auditClient.AuditAsync(
                AuditRequest.AsViewOf(typeof(ItemCollection), "All-Not-Completed")
                .AsEvent("ViewedAllNonCompletedItems")
                .WithNoData());

            return View(items);
        }

#pragma warning disable 1998
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                var createdDocument = await DocumentDBRepository<Item>.CreateItemAsync(item);
                item.Id = createdDocument.Id;

                await this.auditClient.AuditAsync(
                    AuditRequest.AsChangeTo(item, i => i.Id)
                    .AsEvent("CreatedItem")
                    .WithData(item, i => i.Id));

                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Item>.UpdateItemAsync(item.Id, item);

                await this.auditClient.AuditAsync(
                    AuditRequest.AsChangeTo(item, i => i.Id)
                    .AsEvent("EditedItem")
                    .WithData(item, i => i.Id));

                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await this.auditClient.AuditAsync(
                AuditRequest.AsViewOf(item, i => i.Id)
                .AsEvent("ViewedItemDetail")
                .WithData(item, i => i.Id)
                .WithDescription("Viewed item detail for editing"));

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await this.auditClient.AuditAsync(
                AuditRequest.AsViewOf(item, i => i.Id)
                .AsEvent("ViewedItemDetail")
                .WithData(item, i => i.Id)
                .WithDescription("Viewed item detail for deleting"));

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind(Include = "Id")] string id)
        {
            await DocumentDBRepository<Item>.DeleteItemAsync(id);

            await this.auditClient.AuditAsync(
                AuditRequest.AsActionOn(typeof(Item), id)
                .AsEvent("DeletedItem")
                .WithNoData());

            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);

            var auditList = await this.auditReadClient.GetAuditItemsListAsync(
                typeof(Item).Name, id, 100, string.Empty);

            await this.auditClient.AuditAsync(
                AuditRequest.AsViewOf(item, i => i.Id)
                .AsEvent("ViewedItemDetail")
                .WithData(item, i => i.Id)
                .WithDescription("Viewed item detail"));

            return View(new ItemDetail(item, auditList));
        }
    }
}
namespace todo.Controllers
{
    using System.Configuration;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using CloudAudit.Client;
    using Models;

    public class ItemController : Controller
    {
        private IAuditClient auditClient;

        public ItemController()
        {
            this.auditClient = new AuditHttpClient(ConfigurationManager.AppSettings["auditservice.http.baseUrl"]);
            ////this.auditClient = new new AuditServiceBusClient();
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);

            await this.auditClient.AuditAsync(
                AuditRequest.AsViewOf(typeof(ItemCollection), "NotCompleted")
                .AsEvent("ItemsViewed")
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
                var insertedDocument = await DocumentDBRepository<Item>.CreateItemAsync(item);
                item.Id = insertedDocument.Id;

                await this.auditClient.AuditAsync(
                    AuditRequest.AsChangeTo(item, i => item.Id)
                    .AsEvent("ItemCreated")
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
                    .AsEvent("ItemEdited")
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
                .AsEvent("ItemDetailsViewed")
                .WithData(item, i => i.Id)
                .WithDescription("Viewed item details on Edit page"));

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
                .AsEvent("ItemDetailsViewed")
                .WithData(item, i => i.Id)
                .WithDescription("Viewed item details on Delete page"));

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind(Include = "Id")] string id)
        {
            await DocumentDBRepository<Item>.DeleteItemAsync(id);

            await this.auditClient.AuditAsync(
                AuditRequest.AsChangeTo(typeof(Item), id)
                .AsEvent("ItemDeleted")
                .WithNoData());

            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);

            await this.auditClient.AuditAsync(
                AuditRequest.AsViewOf(item, i => i.Id)
                .AsEvent("ItemDetailsViewed")
                .WithData(item, i => i.Id));

            return View(item);
        }
    }
}
namespace PostmanExporter.Models
{
    public class Info
    {
        public string _postman_id { get; set; }
        public string name { get; set; }
        public string schema { get; set; }
        public string _exporter_id { get; set; }
    }

    public class BodyOptionsRaw
    {
        public string language { get; set; }
    }

    public class BodyOptions
    {
        public BodyOptions()
        {
            raw = new BodyOptionsRaw();
        }

        public BodyOptionsRaw raw { get; set; }
    }

    public class Body
    {
        public Body()
        {
            options = new BodyOptions();
        }

        public string mode { get; set; }
        public string raw { get; set; }
        public BodyOptions options { get; set; }
    }

    public class Url
    {
        public Url()
        {
            host = new List<string>();
            path = new List<string>();
        }

        public string raw { get; set; }
        public List<string> host { get; set; }
        public List<string> path { get; set; }
    }

    public class Request
    {
        public Request()
        {
            header = new List<object>();
            body = new Body();
            url = new Url();
        }

        public string method { get; set; }
        public List<object> header { get; set; }
        public Body body { get; set; }
        public Url url { get; set; }
    }

    public class Item
    {
        public Item()
        {
            request = new Request();
            response = new List<object>();
        }

        public string name { get; set; }
        public Request request { get; set; }
        public List<object> response { get; set; }
    }

    public class RootObject
    {
        public RootObject()
        {
            info = new Info();
            item = new List<Item>();
        }

        public Info info { get; set; }
        public List<Item> item { get; set; }

        public void AddItem(string name, string fullPath, string Method, string body)
        {
            var item = new Item();

            item.name = name;
            item.request.method = Method;
            item.request.header = new List<object>();

            if (!string.IsNullOrEmpty(body))
                item.request.body = new Body
                {
                    mode = "raw",
                    raw = body,
                    options = new BodyOptions
                    {
                        raw = new BodyOptionsRaw
                        {
                            language = "json"
                        }
                    }
                };

            item.request.url.raw = "{{$localhost}}" + $"/{fullPath}";
            item.request.url.host = new List<string>() { @"{{$localhost}}" };
            item.request.url.path = string.IsNullOrEmpty(fullPath) ? new List<string>() : fullPath.Split("/").Where(x => !string.IsNullOrEmpty(x)).ToList();
            this.item.Add(item);
        }
    }
}

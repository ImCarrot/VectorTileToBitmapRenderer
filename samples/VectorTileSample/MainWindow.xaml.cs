using System.Windows;
using BruTile.Predefined;
using Mapsui.Layers;
using VectorTileToBitmapRenderer;

namespace VectorTileSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly HttpVectorTileSource _httpVectorTileSource;
        readonly ILayer _vectorTileLayer;
        public MainWindow()
        {
            InitializeComponent();

            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OpenStreetMap"});

            _httpVectorTileSource = CreateVectorTileTileSource();
            // FUTURE_httpVectorTileSource = new HttpTileSource (new GlobalSphericalMercator(), "https://tile.mapzen.com/mapzen/vector/v1/all/{z}/{x}/{y}.mvt?api_key=mapzen-tnjqimH");

            _vectorTileLayer = new TileLayer(_httpVectorTileSource) { Opacity = 0.5, Name = "Mapzen vector tiles"};
            // FUTURE: _vectorTileLayer = new VectorTileLayer(_httpVectorTileSource);

            MapControl.Map.Layers.Add(_vectorTileLayer);

            MapsuiLayerList.Initialize(MapControl.Map.Layers);
        }

        public HttpVectorTileSource CreateVectorTileTileSource()
        {
            return new HttpVectorTileSource(
                new GlobalSphericalMercator(),
                  "http://tile.mapzen.com/mapzen/vector/v1/all/{z}/{x}/{y}.mvt?api_key=mapzen-tnjqimH",
                name: "vector tile");
        }

        private void GDI_OnClick(object sender, RoutedEventArgs e)
        {
            _httpVectorTileSource.UseGdi = true;
            _vectorTileLayer.ClearCache();
            MapControl.Refresh();
        }

        private void OpenTK_OnClick(object sender, RoutedEventArgs e)
        {
            _httpVectorTileSource.UseGdi = false;
            _vectorTileLayer.ClearCache();
            MapControl.Refresh();
        }
    }
}

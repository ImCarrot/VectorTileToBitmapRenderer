using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BruTile;
using BruTile.Cache;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Rendering;

namespace VectorTileSample
{
    public class VectorTileLayer : BaseLayer
    {
        private readonly ITileSource _tileSource;
        private readonly MemoryCache<Feature> _tileCache  = new MemoryCache<Feature>();
        private readonly IRenderGetStrategy _renderStrategy;
        private readonly IFetchStrategy _fetchStrategy = new FetchStrategy();
        private readonly TileFetcher _tileFetcher;
        private List<IFeature> _vectorCache = new List<IFeature>();

        public VectorTileLayer(ITileSource tileSource, RenderGetStrategy renderStrategy = null)
        {
            _tileSource = tileSource;
            _tileFetcher = new TileFetcher(_tileSource, _tileCache, 1, 4, _fetchStrategy);
            _tileFetcher.DataChanged += TileFetcherOnDataChanged;
            _tileFetcher.PropertyChanged += TileFetcherOnPropertyChanged;
            _renderStrategy = renderStrategy ?? new RenderGetStrategy();
        }

        private void TileFetcherOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(Busy)) return;
            if (_tileFetcher != null) Busy = _tileFetcher.Busy;
        }

        private void TileFetcherOnDataChanged(object sender, DataChangedEventArgs dataChangedEventArgs)
        {
            
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (_tileSource?.Schema == null) return Enumerable.Empty<IFeature>();

            return _renderStrategy.GetFeatures(box, resolution, _tileSource?.Schema, _tileCache);
        }

        public override BoundingBox Envelope => _tileSource?.Schema?.Extent.ToBoundingBox();

        public override void AbortFetch()
        {
            // method should be removed
        }

        public override void ViewChanged(bool majorChange, BoundingBox extent, double resolution)
        {
            if (Enabled && extent.GetArea() > 0 && _tileFetcher != null && MaxVisible > resolution && MinVisible < resolution)
            {
                _tileFetcher.ViewChanged(extent, resolution);
            }
        }

        public override void ClearCache()
        {
            _tileCache.Clear();
            _vectorCache.Clear();
        }

        public override bool? IsCrsSupported(string crs)
        {
            return string.Equals(ToSimpleEpsgCode(), crs, StringComparison.CurrentCultureIgnoreCase);
        }

        string ToSimpleEpsgCode()
        {
            var startEpsgCode = _tileSource.Schema.Srs.IndexOf("EPSG:", StringComparison.Ordinal);
            if (startEpsgCode < 0) return _tileSource.Schema.Srs;
            return _tileSource.Schema.Srs.Substring(startEpsgCode).Replace("::", ":").Trim();
        }
    }
}

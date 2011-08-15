﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel
{
    public class LayerRegionViewModel : BindableObject
    {
        private LayerRegion _region;
        private string _name;
        private RangeViewModel _zRangeVM;
        private OpticalPropertyViewModel _opticalPropertyVM;
        
        public LayerRegionViewModel(LayerRegion region, string name)
        {
            _region = region;
            if (string.IsNullOrEmpty(name))
            {
                _name = _region.IsAir() ? "Air" : "Tissue";
            }
            else
            {
                _name = name;
            }
            _zRangeVM = new RangeViewModel(_region.ZRange, "mm", "");
            _opticalPropertyVM = new OpticalPropertyViewModel(_region.RegionOP, "mm-1", "");
        }

        //public LayerRegionViewModel(LayerRegion region) : this(region, "")
        //{
        //}

        public LayerRegionViewModel() : this(new LayerRegion(), "")
        {
        }

        public RangeViewModel ZRangeVM
        {
            get { return _zRangeVM; }
            set
            {
                _zRangeVM = value;
                OnPropertyChanged("ZRangeVM");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public OpticalPropertyViewModel OpticalPropertyVM
        {
            get { return _opticalPropertyVM; }
            set
            {
                _opticalPropertyVM = value;
                OnPropertyChanged("OpticalPropertyVM");
            }
        }

        //public LayerRegion GetLayerRegion()
        //{
        //    return _region;
        //}
    }
}
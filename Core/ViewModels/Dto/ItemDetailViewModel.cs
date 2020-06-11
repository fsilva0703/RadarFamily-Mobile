using System;

using RadarFamilyCore.Models;
using RadarFamilyCore.ViewModels.Dto;

namespace RadarFamilyCore.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public DtoUnitTracker Item { get; set; }
        public ItemDetailViewModel(DtoUnitTracker item = null)
        {
            Title = item?.Name;
            Item = item;
        }
    }
}

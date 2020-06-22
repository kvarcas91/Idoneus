using Domain.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Idoneus.ViewModels
{
    public class DetailsViewModel : BindableBase
    {

        private int _descriptionViewType = 0;
        public int DescriptionViewType
        {
            get { return _descriptionViewType; }
            set { SetProperty(ref _descriptionViewType, value); }
        }

        private int _selectedContributorCount = 0;
        public int SelectedContributorCount
        {
            get { return _selectedContributorCount; }
            set { SetProperty(ref _selectedContributorCount, value); }
        }

        private ObservableCollection<Contributor> _contributors;
        public ObservableCollection<Contributor> Contributors
        {
            get { return _contributors; }
            set { SetProperty(ref _contributors, value); }
        }

        private ObservableCollection<Contributor> _selectedContributors;
        public ObservableCollection<Contributor> SelectedContributors
        {
            get { return _selectedContributors; }
            set { SetProperty(ref _selectedContributors, value); SelectedContributorCount = SelectedContributors.Count; Test(); }
        }

        public DetailsViewModel()
        {
            Contributors = new ObservableCollection<Contributor>();
            for (int i = 0; i < 50; i++)
            {
                Contributors.Add(new Contributor
                {
                    FirstName = $"Eduardas{i}",
                    LastName = "Slutas"
                }
               );
            }

            SelectedContributors = new ObservableCollection<Contributor>();
        }

        public void Test()
        {
            foreach (var item in SelectedContributors)
            {
                Debug.WriteLine(item, nameof(item));
            }
        }


    }
}

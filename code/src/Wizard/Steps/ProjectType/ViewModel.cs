﻿using Microsoft.Templates.Core;
using Microsoft.Templates.Wizard.Host;
using Microsoft.Templates.Wizard.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Microsoft.Templates.Wizard.Steps.ProjectType
{
    public class ViewModel : StepViewModel
    {
        public ObservableCollection<ProjectInfoViewModel> ProjectTypes { get; } = new ObservableCollection<ProjectInfoViewModel>();
        public override string PageTitle => Strings.PageTitle;

        public ViewModel(WizardContext context) : base(context)
        {
        }

        private ProjectInfoViewModel _selectedProjectType;
        public ProjectInfoViewModel SelectedProjectType
        {
            get => _selectedProjectType;
            set => SetProperty(ref _selectedProjectType, value);
        }

        public override async Task InitializeAsync()
        {
            ProjectTypes.Clear();

            var projectTypes = Context.TemplatesRepository.GetAll()
                                                            .Where(t => t.GetTemplateType() == TemplateType.Project && !String.IsNullOrWhiteSpace(t.GetProjectType()))
                                                            .Select(t => t.GetProjectType())
                                                            .Distinct()
                                                            .Select(t => new ProjectInfoViewModel(t, Context.TemplatesRepository.GetProjectTypeInfo(t)))
                                                            .ToList();

            ProjectTypes.AddRange(projectTypes);

            var savedProjectType = Context.GetState<ViewModel, string>();

            if (string.IsNullOrEmpty(savedProjectType))
            {
                SelectedProjectType = projectTypes.FirstOrDefault();
            }
            else
            {
                SelectedProjectType = projectTypes.FirstOrDefault(p => p.Name == savedProjectType);
            }

            await Task.FromResult(true);
        }

        public override void SaveState() => Context.SetState(this, SelectedProjectType.Name);

        protected override Page GetPageInternal()
        {
            return new View();
        }
    }
}

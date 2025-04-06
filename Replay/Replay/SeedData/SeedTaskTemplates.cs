using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Container.Account.MTM;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial <see cref="TaskTemplate"/> for the database
    /// </summary>
    /// <author>Thomas Dworschak, Matthias Grafberger</author>
    public class SeedTaskTemplates
    {
        private static TaskTemplateContainer _taskTemplateContainer;
        private static DuedateContainer _duedateContainer;
        private static ContractTypesContainer _contractTypesContainer;
        private static RoleContainer _roleContainer;
        private static DepartmentContainer _departmentContainer;
        private static MakandraProcessContainer _makandraProcessContainer;

        private static TaskTemplateContractTypeContainer _taskTemplateContractTypeContainer;
        private static TaskTemplateDepartmentContainer _taskTemplateDepartmentContainer;
        private static TaskTemplateRoleContainer _taskTemplateRoleContainer;


        /// <summary>
        /// Creates 42 different <see cref="TaskTemplate"/> according to customer requirement
        /// and adds them to the database
        /// For each template, the many to many relations between <see cref="TaskTemplate"/> and <see cref="Role"/>
        /// are created
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static async void InitializeTaskTemplates(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.TaskTemplates.Any())
                {
                    return;
                }

                _taskTemplateContainer = serviceProvider.GetRequiredService<TaskTemplateContainer>();
                _duedateContainer = serviceProvider.GetRequiredService<DuedateContainer>();
                _contractTypesContainer = serviceProvider.GetRequiredService<ContractTypesContainer>();
                _roleContainer = serviceProvider.GetRequiredService<RoleContainer>();
                _departmentContainer = serviceProvider.GetRequiredService<DepartmentContainer>();
                _makandraProcessContainer = serviceProvider.GetRequiredService<MakandraProcessContainer>();

                _taskTemplateContractTypeContainer = serviceProvider.GetRequiredService<TaskTemplateContractTypeContainer>();
                _taskTemplateDepartmentContainer = serviceProvider.GetRequiredService<TaskTemplateDepartmentContainer>();
                _taskTemplateRoleContainer = serviceProvider.GetRequiredService<TaskTemplateRoleContainer>();

                CreateTaskTemplates();
                InitializeTaskTemplatesRoles();
                InitializeTaskTemplatesContractTypes();
                InitializeTaskTemplatesDepartments();
            }
        }

        /// <summary>
        /// Initialize all <see cref="TaskTemplate">-entries
        /// </summary>
        /// <author>Matthias Grafberger, Thomas Dworschak</author>
        public static async void CreateTaskTemplates() {
            string jsonString = "[\n" +
                "  {\n" +
                "    \"Name\": \"Information zu Rahmenbedingungen des Vertrags einholen\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Vertrag erstellen\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Vertrag vor ab neuen Mitarbeitenden per Mail senden\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Vertrag unterschreiben lassen durch Geschäftsführung\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Personalbogen inkl. erforderlicher Unterlagen per Mail an neuen MA zusenden und ablegen\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Unterstützung für Umzug (500 €) ja/nein abfragen\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Unterstützung für Umzug (500 €) an Personal kommunizieren\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Unterlagen in den jeweiligen Monatsordner für Gehaltsabrechnung DSG hochladen (Vertrag, PB,Perso, SV-Ausweis) und in der Übersicht Änderungen einpflegen\",\n" +
                "    \"DuedateID\": 2,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Mail an Ops wegen Vorbereitung/Bestellung Hardware (Laptop, Handy, etc.) und EinrichtungAccounts\",\n" +
                "    \"DuedateID\": 2,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Backoffice bezüglich Informationen und Termin anschreiben\",\n" +
                "    \"DuedateID\": 2,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Buddy anfragen/informieren und Kontakt vermitteln zur Kontaktaufnahme im Vorfeld\",\n" +
                "    \"DuedateID\": 2,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Einladung neuer MA zu anstehenden Teamevents\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Vernetzung über Soziale Medien\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Versand Willkommens-Box (Backoffice)\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Mail zum 1. Tag im Büro, Vorabinformationen (One Pager - FAQs erster Tag), Uhrzeit, Parkmöglichkeiten, Kleidergröße und Ansprechpartnern vor Ort\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Anlegen in der Time Machine, Einrichten Deskbot-Account und erfragen Mailadresse bei Ops\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Einträge Project Hero im bereich \\\"Team\\\": Senior-Peer, Bereich, Festanstellung seit, Position und Position seit\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Klären Zuordnung Monthly Peer mit GL-Runde\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Klären Zuordnung Buddy\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Mail an Backoffice zur genauen Uhrzeit und Kleidergröße\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Mail Ops wegen Ansprechpartner vor Ort\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Mail Buddy wegen Anwesenheit im Büro\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Brief ausdrucken\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Willkommensmail an das Team über alle@ versenden\",\n" +
                "    \"DuedateID\": 3,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Empfang durch Backoffice und People & Culture\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Übergabe Package, Schlüssel, Karte und Hardware\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Vorstellung Buddy\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Begleitung durch Ops bei der Einrichtung PC\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Büroführung, Vorstellung Kolleg*innen und Schlüssel-/Kartenübergabe durch Backoffice\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Gemeinsames Mittagessen mit Buddy\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Erklärung Organisatorisches und fehlende Unterlagen (NachwG, Datenschutz) durch People&Culture\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Einrichtung PC und Accounts abschließen inkl. Erstellung Sicherungskopie mit Ops\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Begrüßung im Weekly\",\n" +
                "    \"DuedateID\": 4,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Angekommen? Erwartungen? Fragen? Onboarding-Card 2. Teil?\",\n" +
                "    \"DuedateID\": 5,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Angekommen? Erwartungen und Zufriedenheit? Verbesserungsvorschläge?\",\n" +
                "    \"DuedateID\": 6,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Termine für Onboarding-Gespräche\",\n" +
                "    \"DuedateID\": 7,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Termine für Probezeit und Festanstellung Trainee\",\n" +
                "    \"DuedateID\": 7,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Angekommen? Perspektiven? Probezeitende\",\n" +
                "    \"DuedateID\": 7,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Create GSuite account for new user\",\n" +
                "    \"Instruction\": \"  * [ ] Have GSuite create a random password. * [ ] Copy the password to the clipboard. In your personal space on GDrive, create a new document with the \\\"Initiale Zugangsdaten\\\" template and add the GSuite credentials to that document. Print it out, put the paper version into an envelope you can hand over to the new employee. Make sure to delete the document in your GDrive. * [ ] Make the user a member in the organizational unit that fits their job best. * [ ] Click 'save'. * [ ] Make the user a member in the groups 'alle' (both) and 'entwickler' if they're a tech employee. * [ ] Let Anne Kolbe from HR know that the mail account is now ready. She will then create an account with Time Machine for the new employee.\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Create Gitlab account for new user\",\n" +
                "    \"Instruction\": \"If the new user is a tech employee, create an account for the user in GitLab (https://code.makandra.de/admin/users/new) and have the password mailed to them.\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Invite new user to card decks: dev, orga, besprechungsprotokolle, zammad, makandra operations, makandra projektvorstellungen, makandra-projektleitung-curriculum\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  },\n" +
                "  {\n" +
                "    \"Name\": \"Request Slack invite for new user\",\n" +
                "    \"Instruction\": \"Request an invite in Slack via the Top-Right-Button that says `makandra` and fill out the form. The request will be auto-approved because the e-mail ends with `@makandra.de`\",\n" +
                "    \"DuedateID\": 1,\n" +
                "    \"DefaultResponsible\": \"Vorgangsverantwortlicher\",\n" +
                "    \"Archived\": false,\n" +
                "    \"MakandraProcessId\": 1\n" +
                "  }\n" +
                "]";
                        
            _taskTemplateContainer.Import(_duedateContainer, _makandraProcessContainer, jsonString);
        }

        /// <summary>
        /// Initialize all <see cref="TaskTemplateRole">-entries
        /// </summary>
        /// <author>Matthias Grafberger, Thomas Dworschak</author>
        public static async void InitializeTaskTemplatesRoles() {
            int personal = _roleContainer.GetRoleFromName("Personal").Result.Id;
            int it = _roleContainer.GetRoleFromName("IT").Result.Id;
            int admin = _roleContainer.GetRoleFromName("Administrator").Result.Id;

            string jsonString = "[\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 1,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 2,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 3,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 4,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 5,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 6,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 7,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 8,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 9,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 10,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 11,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 12,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 13,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 14,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 15,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 16,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 17,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 18,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 19,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 20,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 21,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 22,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 23,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 24,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 25,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 26,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 27,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 28,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 29,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 30,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 31,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 32,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 33,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 34,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 35,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 36,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 37,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 38,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 39,\n" +
                "    \"RoleID\": " + it + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 40,\n" +
                "    \"RoleID\": " + it + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 41,\n" +
                "    \"RoleID\": " + it + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 42,\n" +
                "    \"RoleID\": " + it + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 1,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 2,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 3,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 4,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 5,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 6,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 7,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 8,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 9,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 10,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 11,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 12,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 13,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 14,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 15,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 16,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 17,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 18,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 19,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 20,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 21,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 22,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 23,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 24,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 25,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 26,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 27,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 28,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 29,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 30,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 31,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 32,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 33,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 34,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 35,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 36,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 37,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 38,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 39,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 40,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 41,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"TaskTemplateID\": 42,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  }\n" +
                "]";
            
            _taskTemplateRoleContainer.Import(_taskTemplateContainer, _roleContainer, jsonString);
        }

        /// <summary>
        /// Initialize all <see cref="TaskTemplateContractType">-entries
        /// </summary>
        /// <author>Matthias Grafberger, Thomas Dworschak</author>
        public static async void InitializeTaskTemplatesContractTypes() {
            for (int i = 1; i <= 4; i++) {
                string jsonString = "[\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 1,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 2,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 3,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 4,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 5,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 6,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 7,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 8,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 9,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 10,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 11,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 12,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 13,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 14,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 15,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 16,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 17,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 18,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 19,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 20,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 21,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 22,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 23,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 24,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 25,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 26,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 27,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 28,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 29,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 30,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 31,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 32,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 33,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 34,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 35,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 36,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 37,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 38,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 39,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 40,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 41,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 42,\n" +
                    "    \"ContractTypeID\": " + i + "\n" +
                    "  }\n" +
                    "]";
                
                _taskTemplateContractTypeContainer.Import(_taskTemplateContainer, _contractTypesContainer, jsonString);
            }
        }

        /// <summary>
        /// Initialize all <see cref="TaskTemplateDepartment">-entries
        /// </summary>
        /// <author>Matthias Grafberger, Thomas Dworschak</author>
        public static async void InitializeTaskTemplatesDepartments() {
            for (int i = 1; i <= 6; i++) {
                string jsonString = "[\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 1,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 2,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 3,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 4,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 5,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 6,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 7,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 8,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 9,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 10,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 11,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 12,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 13,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 14,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 15,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 16,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 17,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 18,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 19,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 20,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 21,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 22,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 23,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 24,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 25,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 26,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 27,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 28,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 29,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 30,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 31,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 32,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 33,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 34,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 35,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 36,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 37,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 38,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 39,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 40,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 41,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"TaskTemplateID\": 42,\n" +
                    "    \"DepartmentID\": " + i + "\n" +
                    "  }\n" +
                    "]";

                _taskTemplateDepartmentContainer.Import(_taskTemplateContainer, _departmentContainer, jsonString);
            }
        }
    }
    
}
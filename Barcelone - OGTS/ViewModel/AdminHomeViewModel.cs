using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class AdminHomeViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        public ICommand GoToConventionAgreementView { get; set; }
        public ICommand GoToOrganigramView { get; set; }
        public ICommand GoToPersonelView { get; set; }
        public ICommand GoToAddEmployeeView { get; set; }
        public ICommand Exploitation { get; set; }
        
        #endregion

        #region Properties

        private string _convention;

        public string Convention
        {
            get { return _convention; }
            set { _convention = value; OnPropertyChanged("Convention"); }
        }

        private Boolean isEnabled;

        public Boolean IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        private string _status;

        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("Status"); }
        }
        public ICollectionView cas { get; private set; }
        public ICollectionView Organigrams { get; private set; }
        private ICollectionView _employeeView;
        public ICollectionView EmployeesView 
        {
            get
            {
                return _employeeView;
            }
            set
            {
                _employeeView = value;
                OnPropertyChanged("EmployeesView");
            }
        }
        #endregion

        #region Constructor
        public AdminHomeViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            GoToConventionAgreementView = new Command(param => PushConventionAgreementView(), param => true);
            GoToOrganigramView = new Command(param => PushOrganigramView(), param => true);
            GoToPersonelView = new Command(param => PushPersonelView(), param => true);
            GoToAddEmployeeView = new Command(param => PushAddWorkerView(), param => true);
            Exploitation = new Command(param => PutInExploitation(), param => true);
            GetEmployees();

            IsEnabled = true;
            Status = "Pas encore en exploitation";
            GetStatus();

            Convention = @"Concernant les « congés légaux » :
- Règles d’acquisition des droits à congés :
- au total 25 jours ouvrés (avec la semaine allant du lundi
au vendredi inclus) sur 12 mois de travail effectués à
temps plein sur l’année de référence allant du 1er
juin de l’année précédente au 31 mai de l’année en
cours,
- soit une acquisition de 2,08 (25 jours / 12 mois) jours par
mois calendaire (quelque soit le nombre de jours
travaillés) pendant la période de référence
- congés composés de 4 semaines de « congé principal » et
d’une semaine « la 5ème semaine »
- Règles de prise de congés :
- les congés légaux peuvent être pris pendant les 12 mois
de l’année de référence qui suit celle d’acquisition.
- les congés légaux acquis depuis le début de l’année de
référence ne peuvent être pris par anticipation.
- L’employeur doit accorder à chaque salarié, au titre du
congé principal, au moins 10 jours de congés
consécutifs dans la période légale allant du 1er mai au
31 octobre ; les 10 jours restant du congé principal,
peuvent être pris en dehors de cette période légale.
- La durée du congé principal pris en une seule fois ne
peut en principe excéder 20 jours ouvrés.
- Les 4 semaines du congé principal sont à prendre par
semaine entière.
- La 5ème semaine ne peut être accolée au congé
principal.
- La 5ème semaine peut être prise dans la période légale
allant du 1er mai au 31 octobre, ou en dehors de cette
période.
- La 5ème semaine peut être prise par journée entière.
- Les congés légaux non pris sont perdus, sauf s’ils sont
placés, avant la fin de la période de prise de ces congés,
sur le compte épargne temps (CET) du salarié dans
la limite de 5 jours.
- Les congés principaux nécessitent un délai de
prévenance de 3 mois calendaires minimum (délai entre
la date de soumission au RH de la demande de congés
du salarié et la date de début de ses congés). Si le
salarié désire prendre 2 semaines ou plus de congés
principaux à partir du 1er mai, il doit en faire la
demande au plus tard fin janvier.
- les jours de la 5ème semaine nécessitent un délai de
prévenance de 5 jours ouvrés.
- Concernant les « congés d’ancienneté » :
- Règles d’acquisition des droits à congés :
- 1 jour ouvré acquis après 2 ans d’ancienneté
- 2 jours ouvrés acquis après 5 ans d’ancienneté
- 3 jours ouvrés acquis après 10 ans d’ancienneté
- 4 jours ouvrés acquis après 15 ans d’ancienneté
- congés acquis, quelque soit la position du salarié, le 1er
jour du mois incluant la date anniversaire d’entrée dans
l’entreprise
- Règles de prise de congés :
- Doivent être pris à partir du 1er jour du mois incluant la
date anniversaire d’entrée dans l’entreprise et avant la
fin du mois qui précède celui incluant la date
anniversaire suivante.
- Les congés d’ancienneté non pris sont perdus, sauf s’ils
sont placés, avant la fin de la période de prise de ces

congés, sur le compte épargne temps (CET) du
salarié.
- Ils doivent être pris par journée entière
- Ils peuvent être accolés à d’autres types de congés
- ils nécessitent un délai de prévenance de 5 jours ouvrés
(délai entre la date de la demande de congés et la date
de début des congés).
- Concernant les « congés supplémentaires » :
- Règles d’acquisition des droits à congés :
- 1 jour ouvré acquis lorsque 3 ou 4 jours de congés
principaux (congés légaux sans la 5ème semaine) sont
pris en dehors de la période allant du 1er mai au 31
octobre
- 2 jours ouvrés acquis lorsqu’au moins 5 jours de congés
principaux (congés légaux sans la 5ème semaine) sont
pris en dehors de la période allant du 1er mai au 31
octobre
- Congés acquis quelque soit l’ancienneté du salarié
- Règles de prise de congés :
- les congés supplémentaires peuvent être pris pendant
les 12 mois de l’année de référence qui suit celle
d’acquisition.
- Les congés supplémentaires non pris sont perdus, sauf
s’ils sont placés, avant la fin de la période de prise de
ces congés, sur le compte épargne temps (CET) du
salarié.
- Ils doivent être pris par journée entière
- Ils peuvent être accolés à d’autres types de congés
- ils nécessitent un délai de prévenance de 5 jours ouvrés
(délai entre la date de la demande de congés et la date
de début des congés).
NB : Pour les I.C., contractualisés au forfait jours, ils
viennent en déduction des 217 jours de travail du forfait, ce
qui ramène le nombre total de jours travaillés dans l’année
civile à 215 ou 216 jours.
- Concernant les « repos forfait » :
- Règles d’acquisition des droits à congés :
- pour les ingénieurs et cadres, bénéficiant du régime de
travail au forfait jours, il s’agit d’un complément de
jours de congés (ajoutés aux congés légaux et congés
supplémentaires) pour que le nombre forfaitaire de
jours travaillés dans l’année civile soit de 217 jours
ouvrés, une fois déduite la journée de solidarité mais
hors prise en compte des congés d’ancienneté.
- Le nombre de jours de repos forfaitaire dépend du
nombre de jours de congés supplémentaires du salarié,
du nombre de samedis et dimanches et du nombre de
jours fériés dans l’année civile, soit la formule de calcul
suivante :
[365 – (nb samedis + nb dimanches + nb jours fériés
coïncidant avec des jours ouvrés + nb jours congés
légaux + nb jours congés supplémentaires)] – 217 jours
du forfait
- pour le salarié entrant dans l’entreprise en cours
d’année civile, le nombre de jours de repos forfait est
calculé en fonction de la durée de présence sur l’année
civile en cours.
- Les jours de repos forfait sont acquis le 1er janvier de
chaque année.
- Règles de prise de congés :
- Doivent être pris entre le 1er janvier et le 31 décembre
de chaque année
- Les jours de repos non pris au 31 décembre sont perdus,
sauf s’ils sont placés, avant la fin de la période de prise
de ces congés, sur le compte épargne temps (CET) du
salarié.
- Ils doivent être pris par journée entière
- Ils peuvent être accolés à d’autres types de congés
- En cas de départ en cours d’année, les jours de repos
sont dus au prorata du temps de présence dan l’année
civile (excepté ceux qui ont été déjà pris)
- ils nécessitent un délai de prévenance de 5 jours ouvrés
(délai entre la date de la demande de congés et la date
de début des congés).
- Concernant les « ponts et fermeture d’entreprise » :
- Règles d’acquisition des droits à congés :
- 2 jours de ponts (un pont est une journée située entre 2
jours fériés ou entre un jour férié et un samedi ou
dimanche) sont offerts chaque année civile par
l’entreprise.
- Les salariés, prévoyant des congés sur une période
englobant un pont, doivent fractionner la pose de leurs
dates de prises de congés de part et d’autre de ce pont.
- La fermeture de la semaine 52 de l’année civile est
instituée par l’entreprise.
- Si la semaine 52 ne comporte que 4 jours ouvrés, la
5ème journée sera prise le 2 janvier de l’année suivante.
- Les salariés, prévoyant des congés sur une période
englobant la semaine de fermeture, doivent fractionner
la pose de leurs dates de prises de congés de part et
d’autre de cette semaine.
- Règles de prise de congés :
- Le salarié doit poser les jours de « ponts » et de «
fermeture »
- Ils sont soit pris sur les autres types de congés (légaux,
ancienneté, supplémentaires, repos forfait) soit retirer
du CET.
- Compte tenu de leurs spécificités, ils ne nécessitent pas
de délai de prévenance.
- Concernant les « congés de l’année précédente » :
- Règles d’acquisition des droits à congés :
- Les congés non pris, dans l’année de référence selon le
type de congés, sont perdus sauf si le salarié les a placé
à temps sur son compte épargne temps (CET).
- Règles de prise de congés :
- Un salarié ne peut prendre de son compte épargne
temps plus de 10 jours au cours de l’année civile, dont
au plus 5 jours placés dans la même année civile.
- Un compte d’épargne temps ne peut pas être négatif.
- ils nécessitent un délai de prévenance de 5 jours ouvrés
(délai entre la date de la demande de congés et la date
de début des congés).
- Concernant les « congés sans solde » :
- Règles d’acquisition des droits à congés :
- aucune.
- Règles de prise de congés :
- Lorsque le cumul des congés acquis non pris (tous types
confondus) avec ceux placés sur le compte épargne
temps sont insuffisants,
- Des congés sans solde ne peuvent être pris qu’à titre
exceptionnel soit pour soigner un enfant malade soit
pour certains évènements familiaux (cf définitions au
§5) :
- mariage du salarié : 5 jours ouvrés,
- mariage du salarié : 5 jours ouvrés,
- mariage d’un enfant du salarié : 2 jours ouvrés
- naissance d’un enfant du salarié ou adoption d’un
enfant par le salarié : 3 jours ouvrés
- décès du conjoint : 3 jours ouvrés
- décès du père, de la mère ou d’un enfant : 2 jours
ouvrés,
- décès du frère ou de la soeur : 1 jour ouvré,
- décès d’un beau-parent : 1 jour ouvré,
- décès d’un grand-parent : 1 jour ouvré,
- décès d’un petit-enfant : 1 jour ouvré.
- Compte tenu de leurs spécificités, ils ne nécessitent pas
de délai de prévenance.";
        }
        #endregion

        private void GetEmployees()
        {
            DbHandler.Instance.OpenConnection();

            try
            {
                List<Employee> employees = new List<Employee>();
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select matricule, firstname, lastname from employee;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        Employee tmp = new Employee
                        {
                            Matricule = result[0].ToString(),
                            Firstname = result[1].ToString(),
                            Lastname = result[2].ToString()
                        };
                        employees.Add(tmp);
                    }

                    EmployeesView = CollectionViewSource.GetDefaultView(employees);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }

        private void GetStatus()
        {
            DbHandler.Instance.OpenConnection();

            try
            {

                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select first_operating_date from organizationchart;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        if (result[0].ToString() != "")
                        {
                            Status = "En exploitation (" + result[0].ToString().Substring(0, 10) + ")";
                            IsEnabled = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }


        private void PushConventionAgreementView()
        {
            Switcher.Switch(new CollectiveAgreementView());
        }

        private void PushOrganigramView()
        {
            Switcher.Switch(new OrganigramView());
        }

        private void PushPersonelView()
        {
            Switcher.Switch(new CollectiveAgreementView());
        }
        private void PushAddWorkerView()
        {
            Switcher.Switch(new AddWorker());
        }

        private void PutInExploitation()
        {
            Status = "En exploitation (" + DateTime.Today.ToShortDateString() + ")";

            DbHandler.Instance.OpenConnection();

            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("insert into organizationchart (first_operating_date) VALUES(date '" + DateTime.Today.ToShortDateString() + "');");
                IsEnabled = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }
        
        #endregion

    }
}

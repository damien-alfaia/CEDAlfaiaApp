using Newtonsoft.Json;
using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.Code;
using CEDAlfaiaApp.Core;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CEDAlfaiaApp.Controllers
{
	public class PronosticsController : CEDAlfaiaAppControllerBase
	{
		/*public ActionResult Clients()
		{
            var concours = CEDAlfaiaAppWebService.GetService().GarageService.GetClients();
            ViewBag.UserID = this.UserID.Value;
			return View(concours);
		}*/
	/*	[HttpGet]
		public ActionResult SearchConcours()
		{
			SearchConcoursModel model = new SearchConcoursModel();
			return View(model);
		}
		[HttpPost]
		public ActionResult SearchConcours(SearchConcoursModel pModel)
		{
			if (pModel != null)
			{
				pModel.Resultats = CEDAlfaiaAppWebService.GetService().PronosService.SearchConcours(pModel.Recherche, this.UserID.Value);
				return View(pModel);
			}
			return View();
		}*/
        /*
        public ActionResult InscrireConcours(int pConcoursID)
        {
            CEDAlfaiaAppWebService.GetService().PronosService.InscrireUserInConcours(this.UserID.Value, pConcoursID);
            return RedirectToAction("Concours");
        }

		private GroupePronosticsModel _setPhaseGroupeProno(Concours pConcours, PhaseGroupe pPhaseGroupe, string pTitreGroupe, TypeSaisiePronostics pTypeSaisiePronostics, bool pIsScoresMatchs)
		{
			GroupePronosticsModel grpModel = new GroupePronosticsModel(pPhaseGroupe, pTitreGroupe);
			if (!pIsScoresMatchs)
				grpModel.Classement = pPhaseGroupe.ClassementWithPronostics(pConcours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && p.Match.PhaseGroupeID == pPhaseGroupe.ID).ToList());
			else
				grpModel.Classement = pPhaseGroupe.Classement();

			foreach (var match in pPhaseGroupe.Matchs.OrderBy(m => m.Date))
			{
				bool isReadOnly = true;
				switch (pTypeSaisiePronostics)
				{
					case TypeSaisiePronostics.ReadOnly:
						isReadOnly = true;
						break;
					case TypeSaisiePronostics.SaisieAvantDateLimite:
						isReadOnly = pConcours.DateLimiteSaisie < DateTime.Now || match.Date < DateTime.Now;
						break;
					case TypeSaisiePronostics.SaisieAvantDateMatch:
						isReadOnly = match.Date < DateTime.Now;
						break;
					case TypeSaisiePronostics.SaisieOnly:
						isReadOnly = pConcours.CompteUtilisateurID != this.UserID;
						break;
				}

				var newProno = new PronosticsModel(pConcours.ID, match, isReadOnly);
				newProno.SetEquipes(match);
				newProno.GroupeModel = grpModel;
				if (!pIsScoresMatchs)
				{
					var prono = pConcours.Pronostics.Where(p => p.MatchID == match.ID && p.CompteUtilisateurID == this.UserID.Value).FirstOrDefault();

					if (prono == null && DateTime.Now > match.Date)
					{
						SetScore(pConcours.ID.ToString(), match.PhaseGroupeID.Value.ToString(), match.ID.ToString(), match.EquipeAID.Value.ToString(), match.EquipeBID.Value.ToString(), "false", "0", "false", true);
						SetScore(pConcours.ID.ToString(), match.PhaseGroupeID.Value.ToString(), match.ID.ToString(), match.EquipeAID.Value.ToString(), match.EquipeBID.Value.ToString(), "true", "0", "false", true);
						prono = pConcours.Pronostics.Where(p => p.MatchID == match.ID && p.CompteUtilisateurID == this.UserID.Value).FirstOrDefault();
					}

					if (prono != null)
					{
						newProno.ButsA = prono.ButsEquipeDomicile;
						newProno.ButsB = prono.ButsEquipeExterieur;
						newProno.SetResultatsMatch(match, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
					}
				}
				else
				{
					newProno.ButsA = match.ButsEquipeDomicile;
					newProno.ButsB = match.ButsEquipeExterieur;
				}

				grpModel.MatchsPronostics.Add(newProno);
			}
			return grpModel;
		}

		private GroupePronosticsModel _setPhasesFinalesAnciensPronos(Concours pConcours, PhaseFinale pPhaseFinale, string pTitrePhaseFinale, TypeSaisiePronostics pTypeSaisiePronostics, bool pIsScoresMatchs, bool pIsNewPronos, List<GroupePronosticsModel> pModel)
		{
			return null;
		}

		public ActionResult SaisirPronostics(int pConcoursID, string pGroupe = "")
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
            if (concours != null)
            {
                ViewBag.Title = concours.Competition.Libelle;
                if (concours.Competition != null)
                {
                    List<GroupePronosticsModel> model = new List<GroupePronosticsModel>();

                    #region Groupes
                    foreach (var grp in concours.Competition.Groupes.OrderBy(g => g.Lettre))
                        model.Add(_setPhaseGroupeProno(concours, grp, pGroupe, TypeSaisiePronostics.SaisieAvantDateLimite, false));
                    #endregion

                    GroupePronosticsModel grpModel3emes = new GroupePronosticsModel();
                    grpModel3emes.ID = 0;
                    grpModel3emes.Name = "Classement des 3èmes";
                    grpModel3emes.ShortName = grpModel3emes.Name;
                    grpModel3emes.IsChoosen = pGroupe == grpModel3emes.Name;
                    grpModel3emes.Classement = concours.Competition.GetClassement3emesPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID).ToList());
                    model.Add(grpModel3emes);

                    #region Phases finales - Anciens
                    foreach (var grp in concours.Competition.PhasesFinales.OrderByDescending(g => (int)g.TypePhaseFinale))
					{
						GroupePronosticsModel grpModel = new GroupePronosticsModel(grp, pGroupe, false);
						
						foreach (var match in grp.Matchs.OrderBy(m => m.Date))
						{
							var prono = concours.Pronostics.Where(p => p.MatchID == match.ID && !p.IsNouveauProno && p.CompteUtilisateurID == this.UserID.Value).FirstOrDefault();

							PronosticsModel pronoModel = new PronosticsModel(pConcoursID, match, concours.DateLimiteSaisie < DateTime.Now || match.Date < DateTime.Now);
							pronoModel.GroupeModel = grpModel;

							var nbPronosticsGroupes = concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && p.Match.PhaseGroupe != null).Count();
							var nbMatchsGroupes = 0;
							foreach (var g in concours.Competition.Groupes)
								nbMatchsGroupes += g.Matchs.Count();

							if (nbPronosticsGroupes >= nbMatchsGroupes)
							{
								if (match.EquipePossibleDomicile_Place != null && match.EquipePossibleExterieur_Place != null)
								{
									// Premiere phase finale (generation apres les groupes)
									var combinaison3emes = concours.Competition.GetCombinaisonClassementTroisiemes(concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID).ToList());
									var combinaison = concours.Competition.TableauCombinaisons.Where(tc => tc.Combinaisons.Intersect(combinaison3emes).Count() == combinaison3emes.Count).FirstOrDefault();

									var groupe_A = concours.Competition.Groupes.Where(g => match.EquipePossibleDomicile_Groupes.Contains(g.Lettre)).FirstOrDefault();
									var classement_A = groupe_A.ClassementWithPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && p.Match.PhaseGroupeID == groupe_A.ID).ToList());
									var equipe_A_ID = classement_A.ElementAt(match.EquipePossibleDomicile_Place.Value - 1).IDEquipe;
									var equipeA = groupe_A.Equipes.Where(e => e.ID == equipe_A_ID).FirstOrDefault();

									var groupe_B = new PhaseGroupe();
									if (match.EquipePossibleExterieur_Place == 3)
									{
										switch (groupe_A.Lettre)
										{
											case "A":
												groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1A.ToString()).FirstOrDefault();
												break;
											case "B":
												groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1B.ToString()).FirstOrDefault();
												break;
											case "C":
												groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1C.ToString()).FirstOrDefault();
												break;
											case "D":
												groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1D.ToString()).FirstOrDefault();
												break;
										}
									}
									else
									{
										groupe_B = concours.Competition.Groupes.Where(g => match.EquipePossibleExterieur_Groupes.Contains(g.Lettre)).FirstOrDefault();
									}
									var classement_B = groupe_B.ClassementWithPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && p.Match.PhaseGroupeID == groupe_B.ID).ToList());
									var equipe_B_ID = classement_B.ElementAt(match.EquipePossibleExterieur_Place.Value - 1).IDEquipe;
									var equipeB = groupe_B.Equipes.Where(e => e.ID == equipe_B_ID).FirstOrDefault();
									if (equipeA != null && equipeB != null)
										pronoModel.SetEquipes(equipeA, equipeB);
								}
								else
								{
									if (grp.TypePhaseFinale == TypePhaseFinale.Quart)
									{
										var nbPronosticsHuitiemes = concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && !p.IsNouveauProno && p.Match.PhaseFinale != null && p.Match.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Huitieme).Count();
										if (nbPronosticsHuitiemes >= (int)TypePhaseFinale.Huitieme)
										{
											var Match_A_ID = match.MatchVainqueurDomicileID.Value;
											var Match_B_ID = match.MatchVainqueurExterieurID.Value;
											var pronosHuitiemes = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Huitieme).FirstOrDefault();
											var match_A = pronosHuitiemes.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
											var match_B = pronosHuitiemes.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
											var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
											var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
											if (equipeA != null && equipeB != null)
												pronoModel.SetEquipes(equipeA, equipeB);
										}
										else
											pronoModel.SetLibellesEquipesProbables(match);
									}
									else if (grp.TypePhaseFinale == TypePhaseFinale.Demi)
									{
										var nbPronosticsQuarts = concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && !p.IsNouveauProno && p.Match.PhaseFinale != null && p.Match.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Quart).Count();
										if (nbPronosticsQuarts >= (int)TypePhaseFinale.Quart)
										{
											var Match_A_ID = match.MatchVainqueurDomicileID.Value;
											var Match_B_ID = match.MatchVainqueurExterieurID.Value;
											var pronosQuarts = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Quart).FirstOrDefault();
											var match_A = pronosQuarts.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
											var match_B = pronosQuarts.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
											var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
											var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
											if (equipeA != null && equipeB != null)
												pronoModel.SetEquipes(equipeA, equipeB);
										}
										else
											pronoModel.SetLibellesEquipesProbables(match);
									}
									else if (grp.TypePhaseFinale == TypePhaseFinale.Finale)
									{
										var nbPronosticsDemis = concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && !p.IsNouveauProno && p.Match.PhaseFinale != null && p.Match.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Demi).Count();
										if (nbPronosticsDemis >= (int)TypePhaseFinale.Demi)
										{
											var Match_A_ID = match.MatchVainqueurDomicileID.Value;
											var Match_B_ID = match.MatchVainqueurExterieurID.Value;
											var pronosDemis = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Demi).FirstOrDefault();
											var match_A = pronosDemis.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
											var match_B = pronosDemis.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
											var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
											var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
											if (equipeA != null && equipeB != null)
												pronoModel.SetEquipes(equipeA, equipeB);
										}
										else
											pronoModel.SetLibellesEquipesProbables(match);
									}
								}
							}
							else
							{
								pronoModel.SetLibellesEquipesProbables(match);
							}

							if (prono != null)
							{
								pronoModel.SetScore(prono);
								pronoModel.SetResultatsMatch(match, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
							}

							grpModel.MatchsPronostics.Add(pronoModel);
						}
						model.Add(grpModel);
					}
                    
                    #endregion

                    #region Huitiemes de finales
                    var nombreMatchsGroupes = concours.Competition.AllMatchs.Where(am => am.PhaseGroupeID != null).Count();
                    var nombreMatchsJouesGroupes = concours.Competition.AllMatchs.Where(am => am.PhaseGroupeID != null && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
                    if (nombreMatchsGroupes == nombreMatchsJouesGroupes)
                    {
                        var huitieme = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Huitieme).FirstOrDefault();
                        if (huitieme != null)
                        {
                            GroupePronosticsModel grpModel = new GroupePronosticsModel(huitieme, pGroupe, true);

                            foreach (var m in huitieme.Matchs)
                            {
                                var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == this.UserID.Value && p.IsNouveauProno).FirstOrDefault();

								var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
								pronoModel.SetEquipes(m);
								pronoModel.IsNewProno = true;
								pronoModel.GroupeModel = grpModel;

                                if (prono != null)
                                {
                                    pronoModel.SetScore(prono);
                                    pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                }


                                grpModel.MatchsPronostics.Add(pronoModel);
                            }
                            model.Add(grpModel);
                        }
                    }
                    #endregion
                    #region Quarts de finales
                    var nombreMatchsHuitiemes = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Huitieme).Count();
                    var nombreMatchsJouesHuitiemes = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Huitieme && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
                    if (nombreMatchsHuitiemes == nombreMatchsJouesHuitiemes)
                    {
                        var quarts = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Quart).FirstOrDefault();
                        if (quarts != null)
                        {
							GroupePronosticsModel grpModel = new GroupePronosticsModel(quarts, pGroupe, true);
							foreach (var m in quarts.Matchs)
                            {
                                var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == this.UserID.Value && p.IsNouveauProno).FirstOrDefault();

								var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
								pronoModel.SetEquipes(m);
								pronoModel.IsNewProno = true;
								pronoModel.GroupeModel = grpModel;

                                if (prono != null)
                                {
                                    pronoModel.SetScore(prono);
                                    pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                }


                                grpModel.MatchsPronostics.Add(pronoModel);
                            }
                            model.Add(grpModel);
                        }
                    }
                    #endregion
                    #region Demis finales
                    var nombreMatchsQuarts = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Quart).Count();
                    var nombreMatchsJouesQuarts = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Quart && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
                    if (nombreMatchsQuarts == nombreMatchsJouesQuarts)
                    {
                        var demis = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Demi).FirstOrDefault();
                        if (demis != null)
                        {
							GroupePronosticsModel grpModel = new GroupePronosticsModel(demis, pGroupe, true);
							foreach (var m in demis.Matchs)
                            {
                                var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == this.UserID.Value && p.IsNouveauProno).FirstOrDefault();

								var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
								pronoModel.SetEquipes(m);
								pronoModel.IsNewProno = true;
								pronoModel.GroupeModel = grpModel;

								if (prono != null)
                                {
                                    pronoModel.SetScore(prono);
                                    pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                }

								grpModel.MatchsPronostics.Add(pronoModel);
                            }
                            model.Add(grpModel);
                        }
                    }
                    #endregion
                    #region Finale
                    var nombreMatchsDemis = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Demi).Count();
                    var nombreMatchsJouesDemis = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Demi && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
                    if (nombreMatchsJouesDemis == nombreMatchsDemis)
                    {
                        var finale = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Finale).FirstOrDefault();
                        if (finale != null)
                        {
							GroupePronosticsModel grpModel = new GroupePronosticsModel(finale, pGroupe, true);
							foreach (var m in finale.Matchs)
                            {
                                var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == this.UserID.Value && p.IsNouveauProno).FirstOrDefault();

								var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
								pronoModel.SetEquipes(m);
								pronoModel.IsNewProno = true;
								pronoModel.GroupeModel = grpModel;

                                if (prono != null)
                                {
                                    pronoModel.SetScore(prono);
                                    pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                }


                                grpModel.MatchsPronostics.Add(pronoModel);
                            }
                            model.Add(grpModel);
                        }
                    }
                    #endregion

                    return View(model);
                }
            }
            return RedirectToAction("Concours");
        }

        public ActionResult ScoresMatch(int pConcoursID, string pGroupe = "")
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
			if (concours.Competition != null)
			{
				List<GroupePronosticsModel> model = new List<GroupePronosticsModel>();

				#region Groupes
				foreach (var grp in concours.Competition.Groupes.OrderBy(g => g.Lettre))
					model.Add(_setPhaseGroupeProno(concours, grp, pGroupe, TypeSaisiePronostics.SaisieOnly, true));
                #endregion

                GroupePronosticsModel grpModel3emes = new GroupePronosticsModel();
                grpModel3emes.ID = 0;
                grpModel3emes.Name = "Classement des 3èmes";
                grpModel3emes.ShortName = grpModel3emes.Name;
                grpModel3emes.IsChoosen = pGroupe == grpModel3emes.Name;
                grpModel3emes.Classement = concours.Competition.GetClassement3emes().ToList();
                model.Add(grpModel3emes);

                #region Phases finales
                foreach (var grp in concours.Competition.PhasesFinales.OrderByDescending(g => (int)g.TypePhaseFinale))
				{
					GroupePronosticsModel grpModel = new GroupePronosticsModel();
					grpModel.ID = grp.ID;
					grpModel.TypePhaseFinale = grp.TypePhaseFinale;
					switch (grp.TypePhaseFinale)
					{
						case TypePhaseFinale.TrenteDeuxieme:
							grpModel.Name = grpModel.ShortName = "32° de finale";
							break;
						case TypePhaseFinale.Seizieme:
							grpModel.Name = grpModel.ShortName = "16° de finale";
							break;
						case TypePhaseFinale.Huitieme:
							grpModel.Name = grpModel.ShortName = "Huitieme de finale";
							break;
						case TypePhaseFinale.Quart:
							grpModel.Name = grpModel.ShortName = "Quart de finales";
							break;
						case TypePhaseFinale.Demi:
							grpModel.Name = grpModel.ShortName = "Demi-finale";
							break;
						case TypePhaseFinale.Finale:
							grpModel.Name = grpModel.ShortName = "Finale";
							break;
					}
					grpModel.IsChoosen = pGroupe == grpModel.Name;

					foreach (var match in grp.Matchs.OrderBy(m => m.Date))
					{
						PronosticsModel pronoModel = new PronosticsModel(pConcoursID, match, concours.DateLimiteSaisie < DateTime.Now || match.Date < DateTime.Now);
						pronoModel.GroupeModel = grpModel;
                        pronoModel.IsReadOnly = concours.CompteUtilisateurID != this.UserID;

                        var nbScoresGroupes = 0;
						var nbMatchsGroupes = 0;
						foreach (var g in concours.Competition.Groupes)
							nbScoresGroupes += g.Matchs.Where(m => m.ButsEquipeDomicile != null && m.ButsEquipeExterieur != null).Count();
						foreach (var g in concours.Competition.Groupes)
							nbMatchsGroupes += g.Matchs.Count();

						if (nbScoresGroupes >= nbMatchsGroupes)
						{
							if (match.EquipePossibleDomicile_Place != null && match.EquipePossibleExterieur_Place != null)
							{
								// Premiere phase finale (generation apres les groupes)
								var combinaison3emes = concours.Competition.GetCombinaisonClassementTroisiemes();
								var combinaison = concours.Competition.TableauCombinaisons.Where(tc => tc.Combinaisons.Intersect(combinaison3emes).Count() == combinaison3emes.Count).FirstOrDefault();

								var groupe_A = concours.Competition.Groupes.Where(g => match.EquipePossibleDomicile_Groupes.Contains(g.Lettre)).FirstOrDefault();
								var classement_A = groupe_A.Classement().ToList();
								var equipe_A_ID = classement_A.ElementAt(match.EquipePossibleDomicile_Place.Value - 1).IDEquipe;
								var equipeA = groupe_A.Equipes.Where(e => e.ID == equipe_A_ID).FirstOrDefault();

								var groupe_B = new PhaseGroupe();
								if (match.EquipePossibleExterieur_Place == 3)
								{
									switch (groupe_A.Lettre)
									{
										case "A":
											groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1A.ToString()).FirstOrDefault();
											break;
										case "B":
											groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1B.ToString()).FirstOrDefault();
											break;
										case "C":
											groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1C.ToString()).FirstOrDefault();
											break;
										case "D":
											groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1D.ToString()).FirstOrDefault();
											break;
									}
								}
								else
								{
									groupe_B = concours.Competition.Groupes.Where(g => match.EquipePossibleExterieur_Groupes.Contains(g.Lettre)).FirstOrDefault();
								}
								var classement_B = groupe_B.Classement().ToList();
								var equipe_B_ID = classement_B.ElementAt(match.EquipePossibleExterieur_Place.Value - 1).IDEquipe;
								var equipeB = groupe_B.Equipes.Where(e => e.ID == equipe_B_ID).FirstOrDefault();

								if (equipeA != null && equipeB != null)
									pronoModel.SetEquipes(equipeA, equipeB);
							}
							else
							{
								if (grp.TypePhaseFinale == TypePhaseFinale.Quart)
								{
									var nbScoresHuitiemes = 0;
									foreach (var pf in concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Huitieme))
										nbScoresHuitiemes += pf.Matchs.Where(m => m.ButsEquipeDomicile != null && m.ButsEquipeExterieur != null).Count();

									if (nbScoresHuitiemes >= (int)TypePhaseFinale.Huitieme)
									{
										var Match_A_ID = match.MatchVainqueurDomicileID.Value;
										var Match_B_ID = match.MatchVainqueurExterieurID.Value;
										var pronosHuitiemes = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Huitieme).FirstOrDefault();
										var match_A = pronosHuitiemes.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
										var match_B = pronosHuitiemes.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
										var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
										var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
										if (equipeA != null && equipeB != null)
											pronoModel.SetEquipes(equipeA, equipeB);
									}
									else
										pronoModel.SetLibellesEquipesProbables(match);
								}
								else if (grp.TypePhaseFinale == TypePhaseFinale.Demi)
								{
									var nbScoresQuarts = 0;
									foreach (var pf in concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Quart))
										nbScoresQuarts += pf.Matchs.Where(m => m.ButsEquipeDomicile != null && m.ButsEquipeExterieur != null).Count();

									if (nbScoresQuarts >= (int)TypePhaseFinale.Quart)
									{
										var Match_A_ID = match.MatchVainqueurDomicileID.Value;
										var Match_B_ID = match.MatchVainqueurExterieurID.Value;
										var pronosQuarts = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Quart).FirstOrDefault();
										var match_A = pronosQuarts.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
										var match_B = pronosQuarts.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
										var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
										var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
										if (equipeA != null && equipeB != null)
											pronoModel.SetEquipes(equipeA, equipeB);
									}
									else
										pronoModel.SetLibellesEquipesProbables(match);
								}
								else if (grp.TypePhaseFinale == TypePhaseFinale.Finale)
								{
									var nbScoresQuarts = 0;
									foreach (var pf in concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Demi))
										nbScoresQuarts += pf.Matchs.Where(m => m.ButsEquipeDomicile != null && m.ButsEquipeExterieur != null).Count();

									if (nbScoresQuarts >= (int)TypePhaseFinale.Demi)
									{
										var Match_A_ID = match.MatchVainqueurDomicileID.Value;
										var Match_B_ID = match.MatchVainqueurExterieurID.Value;
										var pronosDemis = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Demi).FirstOrDefault();
										var match_A = pronosDemis.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
										var match_B = pronosDemis.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
										var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
										var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
										if (equipeA != null && equipeB != null)
											pronoModel.SetEquipes(equipeA, equipeB);
									}
									else
										pronoModel.SetLibellesEquipesProbables(match);
								}
							}
						}
						else
							pronoModel.SetLibellesEquipesProbables(match);

						if (match != null)
							pronoModel.SetScore(match);
						
						grpModel.MatchsPronostics.Add(pronoModel);
					}
					model.Add(grpModel);
				}
				#endregion 

				return View(model);
            }
            return RedirectToAction("Concours");
        }

        public ActionResult Pronostics(int pConcoursID, int pUserID, string pGroupe = "")
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
            if (concours != null)
            {
                ViewBag.Title = concours.Competition.Libelle;
                if (concours.Competition != null)
                {
                    List<GroupePronosticsModel> model = new List<GroupePronosticsModel>();

                    #region Groupes
                    foreach (var grp in concours.Competition.Groupes.OrderBy(g => g.Lettre))
                        model.Add(_setPhaseGroupeProno(concours, grp, pGroupe, TypeSaisiePronostics.ReadOnly, false));
                    #endregion

                    GroupePronosticsModel grpModel3emes = new GroupePronosticsModel();
                    grpModel3emes.ID = 0;
                    grpModel3emes.Name = "Classement des 3èmes";
                    grpModel3emes.ShortName = grpModel3emes.Name;
                    grpModel3emes.IsChoosen = pGroupe == grpModel3emes.Name;
                    grpModel3emes.Classement = concours.Competition.GetClassement3emesPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID).ToList());
                    model.Add(grpModel3emes);

                    #region Phases finales
                    foreach (var grp in concours.Competition.PhasesFinales.OrderByDescending(g => (int)g.TypePhaseFinale))
                    {
                        GroupePronosticsModel grpModel = new GroupePronosticsModel(grp, pGroupe, false);
						foreach (var match in grp.Matchs.OrderBy(m => m.Date))
                        {
                            var prono = concours.Pronostics.Where(p => p.MatchID == match.ID && p.CompteUtilisateurID == pUserID).FirstOrDefault();

                            PronosticsModel pronoModel = new PronosticsModel(pConcoursID, match, true);
							pronoModel.GroupeModel = grpModel;

							var nbPronosticsGroupes = concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID && p.Match.PhaseGroupe != null).Count();
                            var nbMatchsGroupes = 0;
                            foreach (var g in concours.Competition.Groupes)
                                nbMatchsGroupes += g.Matchs.Count();

                            if (nbPronosticsGroupes >= nbMatchsGroupes)
                            {
                                if (match.EquipePossibleDomicile_Place != null && match.EquipePossibleExterieur_Place != null)
                                {
                                    // Premiere phase finale (generation apres les groupes)
                                    var combinaison3emes = concours.Competition.GetCombinaisonClassementTroisiemes(concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID).ToList());
                                    var combinaison = concours.Competition.TableauCombinaisons.Where(tc => tc.Combinaisons.Intersect(combinaison3emes).Count() == combinaison3emes.Count).FirstOrDefault();

                                    var groupe_A = concours.Competition.Groupes.Where(g => match.EquipePossibleDomicile_Groupes.Contains(g.Lettre)).FirstOrDefault();
                                    var classement_A = groupe_A.ClassementWithPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID && p.Match.PhaseGroupeID == groupe_A.ID).ToList());
                                    var equipe_A_ID = classement_A.ElementAt(match.EquipePossibleDomicile_Place.Value - 1).IDEquipe;
                                    var equipeA = groupe_A.Equipes.Where(e => e.ID == equipe_A_ID).FirstOrDefault();

                                    var groupe_B = new PhaseGroupe();
                                    if (match.EquipePossibleExterieur_Place == 3)
                                    {
                                        switch (groupe_A.Lettre)
                                        {
                                            case "A":
                                                groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1A.ToString()).FirstOrDefault();
                                                break;
                                            case "B":
                                                groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1B.ToString()).FirstOrDefault();
                                                break;
                                            case "C":
                                                groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1C.ToString()).FirstOrDefault();
                                                break;
                                            case "D":
                                                groupe_B = concours.Competition.Groupes.Where(g => g.Lettre == combinaison.Adversaire1D.ToString()).FirstOrDefault();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        groupe_B = concours.Competition.Groupes.Where(g => match.EquipePossibleExterieur_Groupes.Contains(g.Lettre)).FirstOrDefault();
                                    }
                                    var classement_B = groupe_B.ClassementWithPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID && p.Match.PhaseGroupeID == groupe_B.ID).ToList());
                                    var equipe_B_ID = classement_B.ElementAt(match.EquipePossibleExterieur_Place.Value - 1).IDEquipe;
                                    var equipeB = groupe_B.Equipes.Where(e => e.ID == equipe_B_ID).FirstOrDefault();
                                    if (equipeA != null && equipeB != null)
										pronoModel.SetEquipes(equipeA, equipeB);
                                }
                                else
                                {
                                    if (grp.TypePhaseFinale == TypePhaseFinale.Quart)
                                    {
                                        var nbPronosticsHuitiemes = concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID && p.Match.PhaseFinale != null && p.Match.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Huitieme).Count();
                                        if (nbPronosticsHuitiemes >= (int)TypePhaseFinale.Huitieme)
                                        {
                                            var Match_A_ID = match.MatchVainqueurDomicileID.Value;
                                            var Match_B_ID = match.MatchVainqueurExterieurID.Value;
                                            var pronosHuitiemes = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Huitieme).FirstOrDefault();
                                            var match_A = pronosHuitiemes.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
                                            var match_B = pronosHuitiemes.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
                                            var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
                                            var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
                                            if (equipeA != null && equipeB != null)
												pronoModel.SetEquipes(equipeA, equipeB);
                                        }
                                        else
											pronoModel.SetLibellesEquipesProbables(match);
                                    }
                                    else if (grp.TypePhaseFinale == TypePhaseFinale.Demi)
                                    {
                                        var nbPronosticsQuarts = concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID && p.Match.PhaseFinale != null && p.Match.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Quart).Count();
										if (nbPronosticsQuarts >= (int)TypePhaseFinale.Quart)
										{
											var Match_A_ID = match.MatchVainqueurDomicileID.Value;
											var Match_B_ID = match.MatchVainqueurExterieurID.Value;
											var pronosQuarts = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Quart).FirstOrDefault();
											var match_A = pronosQuarts.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
											var match_B = pronosQuarts.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
											var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
											var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
											if (equipeA != null && equipeB != null)
												pronoModel.SetEquipes(equipeA, equipeB);
										}
										else
											pronoModel.SetLibellesEquipesProbables(match);
                                    }
                                    else if (grp.TypePhaseFinale == TypePhaseFinale.Finale)
                                    {
                                        var nbPronosticsDemis = concours.Pronostics.Where(p => p.CompteUtilisateurID == pUserID && p.Match.PhaseFinale != null && p.Match.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Demi).Count();
                                        if (nbPronosticsDemis >= (int)TypePhaseFinale.Demi)
                                        {
                                            var Match_A_ID = match.MatchVainqueurDomicileID.Value;
                                            var Match_B_ID = match.MatchVainqueurExterieurID.Value;
                                            var pronosDemis = model.Where(m => m.TypePhaseFinale == TypePhaseFinale.Demi).FirstOrDefault();
                                            var match_A = pronosDemis.MatchsPronostics.Where(m => m.NumeroMatch == Match_A_ID).FirstOrDefault();
                                            var match_B = pronosDemis.MatchsPronostics.Where(m => m.NumeroMatch == Match_B_ID).FirstOrDefault();
                                            var equipeA = concours.Competition.Equipes.Where(e => e.ID == match_A.VanqueurID).FirstOrDefault();
                                            var equipeB = concours.Competition.Equipes.Where(e => e.ID == match_B.VanqueurID).FirstOrDefault();
                                            if (equipeA != null && equipeB != null)
												pronoModel.SetEquipes(equipeA, equipeB);
                                        }
                                        else
											pronoModel.SetLibellesEquipesProbables(match);
                                    }
                                }
                            }
                            else
								pronoModel.SetLibellesEquipesProbables(match);

                            if (prono != null)
							{
								pronoModel.SetScore(prono);
								pronoModel.SetResultatsMatch(match, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
							}
							
                            grpModel.MatchsPronostics.Add(pronoModel);
                        }
                        model.Add(grpModel);
                    }
					#endregion

					#region Huitiemes de finales
					var nombreMatchsGroupes = concours.Competition.AllMatchs.Where(am => am.PhaseGroupeID != null).Count();
					var nombreMatchsJouesGroupes = concours.Competition.AllMatchs.Where(am => am.PhaseGroupeID != null && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
					if (nombreMatchsGroupes == nombreMatchsJouesGroupes)
					{
						var huitieme = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Huitieme).FirstOrDefault();
						if (huitieme != null)
						{
							GroupePronosticsModel grpModel = new GroupePronosticsModel(huitieme, pGroupe, true);

							foreach (var m in huitieme.Matchs)
							{
								if (m.Date < DateTime.Now)
								{
									var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == pUserID && p.IsNouveauProno).FirstOrDefault();

									var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
									pronoModel.SetEquipes(m);
									pronoModel.IsNewProno = true;
									pronoModel.GroupeModel = grpModel;

                                    if (prono != null)
                                    {
                                        pronoModel.SetScore(prono);
                                        pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                    }


                                    grpModel.MatchsPronostics.Add(pronoModel);
								}
							}
							model.Add(grpModel);
						}
					}
					#endregion
					#region Quarts de finales
					var nombreMatchsHuitiemes = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Huitieme).Count();
					var nombreMatchsJouesHuitiemes = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Huitieme && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
					if (nombreMatchsHuitiemes == nombreMatchsJouesHuitiemes)
					{
						var quarts = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Quart).FirstOrDefault();
						if (quarts != null)
						{
							GroupePronosticsModel grpModel = new GroupePronosticsModel(quarts, pGroupe, true);
							foreach (var m in quarts.Matchs)
							{
								if (m.Date < DateTime.Now)
								{
									var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == pUserID && p.IsNouveauProno).FirstOrDefault();

									var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
									pronoModel.SetEquipes(m);
									pronoModel.IsNewProno = true;
									pronoModel.GroupeModel = grpModel;

                                    if (prono != null)
                                    {
                                        pronoModel.SetScore(prono);
                                        pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                    }


                                    grpModel.MatchsPronostics.Add(pronoModel);
								}
							}
							model.Add(grpModel);
						}
					}
					#endregion
					#region Demis finales
					var nombreMatchsQuarts = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Quart).Count();
					var nombreMatchsJouesQuarts = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Quart && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
					if (nombreMatchsQuarts == nombreMatchsJouesQuarts)
					{
						var demis = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Demi).FirstOrDefault();
						if (demis != null)
						{
							GroupePronosticsModel grpModel = new GroupePronosticsModel(demis, pGroupe, true);
							foreach (var m in demis.Matchs)
							{
								if (m.Date < DateTime.Now)
								{
									var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == pUserID && p.IsNouveauProno).FirstOrDefault();

									var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
									pronoModel.SetEquipes(m);
									pronoModel.IsNewProno = true;
									pronoModel.GroupeModel = grpModel;

                                    if (prono != null)
                                    {
                                        pronoModel.SetScore(prono);
                                        pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                    }


                                    grpModel.MatchsPronostics.Add(pronoModel);
								}
							}
							model.Add(grpModel);
						}
					}
					#endregion
					#region Finale
					var nombreMatchsDemis = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Demi).Count();
					var nombreMatchsJouesDemis = concours.Competition.AllMatchs.Where(am => am.PhaseFinale != null && am.PhaseFinale.TypePhaseFinale == TypePhaseFinale.Demi && am.ButsEquipeDomicile != null && am.ButsEquipeExterieur != null).Count();
					if (nombreMatchsJouesDemis == nombreMatchsDemis)
					{
						var finale = concours.Competition.PhasesFinales.Where(pf => pf.TypePhaseFinale == TypePhaseFinale.Finale).FirstOrDefault();
						if (finale != null)
						{
							GroupePronosticsModel grpModel = new GroupePronosticsModel(finale, pGroupe, true);
							foreach (var m in finale.Matchs)
							{
								if (m.Date < DateTime.Now)
								{
									var prono = concours.Pronostics.Where(p => p.MatchID == m.ID && p.CompteUtilisateurID == pUserID && p.IsNouveauProno).FirstOrDefault();

									var pronoModel = new PronosticsModel(pConcoursID, m, DateTime.Now > m.Date);
									pronoModel.SetEquipes(m);
									pronoModel.IsNewProno = true;
									pronoModel.GroupeModel = grpModel;

                                    if (prono != null)
                                    {
                                        pronoModel.SetScore(prono);
                                        pronoModel.SetResultatsMatch(m, CEDAlfaiaAppWebService.GetService().PronosService.GetPointsPronoForAnUser(prono.ID, this.UserID.Value));
                                    }


                                    grpModel.MatchsPronostics.Add(pronoModel);
								}
							}
							model.Add(grpModel);
						}
					}
					#endregion

					return View(model);
                }
            }
            return RedirectToAction("ClassementConcours", new { pConcoursID = pConcoursID });
        }

        [HttpGet]
        public ActionResult ClassementConcours(int pConcoursID)
        {
            Concours c = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
			ConcoursClassementViewModel model = new ConcoursClassementViewModel()
			{
				ConcoursID = pConcoursID,
                Classement = c.Classement(), 
				ShowAnciensPronos = true,
				ShowGroupes = true,
				ShowNouveauxPronos = true
            };
            if (model != null)
                return View(model);
            return RedirectToAction("Concours");
        }

		[HttpGet]
		public ActionResult GetClassementConcours(string pConcoursID, bool pShowGroupes, bool pShowAnciensPronos, bool pShowNouveauxPronos)
		{
			Concours c = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(Helper.GetIntFromString(pConcoursID).Value);
			ConcoursClassementViewModel model = new ConcoursClassementViewModel()
			{
				ConcoursID = Helper.GetIntFromString(pConcoursID).Value,
				Classement = c.Classement(pShowGroupes, pShowAnciensPronos, pShowNouveauxPronos),
				ShowAnciensPronos = pShowAnciensPronos,
				ShowGroupes = pShowGroupes,
				ShowNouveauxPronos = pShowNouveauxPronos
			};
			if (model != null)
				return PartialView("_ClassementConcours", model);
			return PartialView();
		}

		[HttpGet]
        public ActionResult GetClassement(string pConcoursID, string pGroupeID)
        {
            if (pConcoursID != null && pGroupeID != null)
            {
                int concoursID = Helper.GetIntFromString(pConcoursID).Value;
                int groupeID = Helper.GetIntFromString(pGroupeID).Value;
                return PartialView("_Classement", GetClassementByGroupeID(concoursID, groupeID));
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetClassement3emes(string pConcoursID)
        {
            if (pConcoursID != null)
            {
                int concoursID = Helper.GetIntFromString(pConcoursID).Value;
                return PartialView("_Classement", GetClassement3emes(concoursID));
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetClassement3emesPronostics(string pConcoursID)
        {
            if (pConcoursID != null)
            {
                int concoursID = Helper.GetIntFromString(pConcoursID).Value;
                return PartialView("_Classement", GetClassement3emesPronostics(concoursID));
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetClassementMatch(string pConcoursID, string pGroupeID)
        {
            if (pConcoursID != null && pGroupeID != null)
            {
                int concoursID = Helper.GetIntFromString(pConcoursID).Value;
                int groupeID = Helper.GetIntFromString(pGroupeID).Value;
                return PartialView("_Classement", GetClassementMatchByGroupeID(concoursID, groupeID));
            }
            return PartialView();
        }

        [HttpGet]
        public bool SetScore(string pConcoursID, string pGroupeID, string pMatchID, string pEquipeAID, string pEquipeBID, string pIsExterieur, string pValue, string pIsNewProno, bool pSkipDate = false)
        {
            int userID = this.UserID.Value;
            int concoursID = Helper.GetIntFromString(pConcoursID).Value;
            int matchID = Helper.GetIntFromString(pMatchID).Value;
            int equipeAID = Helper.GetIntFromString(pEquipeAID).Value;
            int equipeBID = Helper.GetIntFromString(pEquipeBID).Value;
            int value = Helper.GetIntFromString(pValue).Value;
            bool isExterieur = Helper.GetBoolFromString(pIsExterieur).Value;
            int groupeID = Helper.GetIntFromString(pGroupeID).Value;
            bool isNewProno = Helper.GetBoolFromString(pIsNewProno).Value;

            CEDAlfaiaAppWebService.GetService().PronosService.SetScore(userID, concoursID, matchID, equipeAID, equipeBID, isExterieur, value, isNewProno, pSkipDate);
            return true;
        }

        [HttpGet]
        public bool SetScorePenalties(string pConcoursID, string pGroupeID, string pMatchID, string pEquipeAID, string pEquipeBID, string pIsExterieur, string pValue, string pIsNewProno)
        {
            int userID = this.UserID.Value;
            int concoursID = Helper.GetIntFromString(pConcoursID).Value;
            int matchID = Helper.GetIntFromString(pMatchID).Value;
            int equipeAID = Helper.GetIntFromString(pEquipeAID).Value;
            int equipeBID = Helper.GetIntFromString(pEquipeBID).Value;
            int value = Helper.GetIntFromString(pValue).Value;
            bool isExterieur = Helper.GetBoolFromString(pIsExterieur).Value;
            int groupeID = Helper.GetIntFromString(pGroupeID).Value;
            bool isNewProno = Helper.GetBoolFromString(pIsNewProno).Value;

            CEDAlfaiaAppWebService.GetService().PronosService.SetScorePenalties(userID, concoursID, matchID, equipeAID, equipeBID, isExterieur, value, isNewProno);
            return true;
        }

        [HttpGet]
        public bool SetScoreMatch(string pConcoursID, string pGroupeID, string pMatchID, string pEquipeAID, string pEquipeBID, string pIsExterieur, string pValue)
        {
            int userID = this.UserID.Value;
            int concoursID = Helper.GetIntFromString(pConcoursID).Value;
            int matchID = Helper.GetIntFromString(pMatchID).Value;
            int equipeAID = Helper.GetIntFromString(pEquipeAID).Value;
            int equipeBID = Helper.GetIntFromString(pEquipeBID).Value;
            int value = Helper.GetIntFromString(pValue).Value;
            bool isExterieur = Helper.GetBoolFromString(pIsExterieur).Value;
            int groupeID = Helper.GetIntFromString(pGroupeID).Value;

            CEDAlfaiaAppWebService.GetService().PronosService.SetScoreMatch(userID, concoursID, matchID, equipeAID, equipeBID, isExterieur, value);
            return true;
        }

        [HttpGet]
        public bool SetScoreMatchPenalties(string pConcoursID, string pGroupeID, string pMatchID, string pEquipeAID, string pEquipeBID, string pIsExterieur, string pValue)
        {
            int userID = this.UserID.Value;
            int concoursID = Helper.GetIntFromString(pConcoursID).Value;
            int matchID = Helper.GetIntFromString(pMatchID).Value;
            int equipeAID = Helper.GetIntFromString(pEquipeAID).Value;
            int equipeBID = Helper.GetIntFromString(pEquipeBID).Value;
            int value = Helper.GetIntFromString(pValue).Value;
            bool isExterieur = Helper.GetBoolFromString(pIsExterieur).Value;
            int groupeID = Helper.GetIntFromString(pGroupeID).Value;

            CEDAlfaiaAppWebService.GetService().PronosService.SetScorePenaltiesMatch(userID, concoursID, matchID, equipeAID, equipeBID, isExterieur, value);
            return true;
        }

        private List<PhaseGroupe.ClassementGroupeModel> GetClassementByGroupeID(int pConcoursID, int pGroupeID)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
            if (concours != null)
            {
                var grp = concours.Competition.Groupes.Where(g => g.ID == pGroupeID).FirstOrDefault();
                if (grp != null)
                    return grp.ClassementWithPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID && p.Match.PhaseGroupeID == grp.ID).ToList());
            }
            return null;
        }
        private List<PhaseGroupe.ClassementGroupeModel> GetClassement3emesPronostics(int pConcoursID)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
            if (concours != null)
                return concours.Competition.GetClassement3emesPronostics(concours.Pronostics.Where(p => p.CompteUtilisateurID == this.UserID).ToList());
            return null;
        }
        private List<PhaseGroupe.ClassementGroupeModel> GetClassement3emes(int pConcoursID)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
            if (concours != null)
                return concours.Competition.GetClassement3emes().ToList();
            return null;
        }
        private List<PhaseGroupe.ClassementGroupeModel> GetClassementMatchByGroupeID(int pConcoursID, int pGroupeID)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
            if (concours != null)
            {
                var grp = concours.Competition.Groupes.Where(g => g.ID == pGroupeID).FirstOrDefault();
                if (grp != null)
                    return grp.Classement().ToList();
            }
            return null;
        }

		public ActionResult InformationsMatch(int pIdMatch)
		{
			var match = CEDAlfaiaAppWebService.GetService().PronosService.GetMatchByID(pIdMatch);
            if (match != null)
                ViewBag.Title = match.EquipeA.Libelle + " vs. " + match.EquipeB.Libelle;
            return View(match);
		}

        public JsonResult GetStatsScoresJson(int pIdConcours, int pIdMatch, string pIsNewProno)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pIdConcours);
            bool isNewProno = Helper.GetBoolFromString(pIsNewProno).Value;
            var model = new InformationsPronosticViewModel() { ListePronostics = concours.Pronostics.Where(p => p.MatchID == pIdMatch && p.IsNouveauProno == isNewProno).ToList() };
           
            return Json(JsonConvert.SerializeObject(model.StatsParScore), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatsVainqueursJson(int pIdConcours, int pIdMatch, string pIsNewProno)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pIdConcours);
            bool isNewProno = Helper.GetBoolFromString(pIsNewProno).Value;
            var model = new InformationsPronosticViewModel() { ListePronostics = concours.Pronostics.Where(p => p.MatchID == pIdMatch && p.IsNouveauProno == isNewProno).ToList() };

            return Json(JsonConvert.SerializeObject(model.StatsParVainqueur), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatsClassementConcours(int pIdConcours)
        {
            var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pIdConcours);
            var classement = concours.ClassementParMatch();

            List<StatsObjectDataClassementItemModel> stats = new List<StatsObjectDataClassementItemModel>();
            int i = 1;
            foreach (var element in classement)
            {
                foreach (var e in element.Value)
                {
                    string nom = e.NomComplet;
                    int place = element.Value.FindIndex(v => v == e) + 1;

                    if (stats.Any(s => s.label == nom))
                    {
                        var stat = stats.Where(s => s.label == nom).FirstOrDefault();
                        if (stat != null)
                            stat.data.Add(new List<int>()
                            {
                                i, place
                            });
                    }
                    else
                    {
                        stats.Add(new StatsObjectDataClassementItemModel()
                        {
                            label = nom,
                            data = new List<List<int>>()
                            {
                                new List<int>
                                {
                                    i, place
                                }
                            }
                        });
                    }
                }
                i++;
            }

            return Json(JsonConvert.SerializeObject(stats), JsonRequestBehavior.AllowGet);

        }

        public ActionResult InformationsPronostic(int pIdConcours, int pIdMatch, bool pIsNewProno = false)
		{
			var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pIdConcours);
			var model = new InformationsPronosticViewModel() { ListePronostics = concours.Pronostics.Where(p => p.MatchID == pIdMatch && p.IsNouveauProno == pIsNewProno).ToList() };
            model.MatchID = pIdMatch;
            model.ConcoursID = pIdConcours;
            model.IsNewProno = pIsNewProno;
            var match = concours.Competition.AllMatchs.Where(m => m.ID == pIdMatch).FirstOrDefault();
            if (match != null)
                ViewBag.Title = match.EquipeA.Libelle + " vs. " + match.EquipeB.Libelle;

            return View(model);
		}

		public ActionResult StatsConcours(int pConcoursID)
		{
			List<GenericStatViewModel> model = new List<GenericStatViewModel>();

			var concours = CEDAlfaiaAppWebService.GetService().PronosService.GetConcoursByID(pConcoursID);
			if (concours != null)
			{
				model.Add(new GenericStatViewModel("StatsNombrePronostics", "pronostics", concours.Pronostics.Count, "panel-info", "fa-soccer-ball-o", concours.Pronostics.GroupBy(g => g.CompteUtilisateur).Distinct().Select(g => new GenericListItemViewModel() { Label = g.Key.Prenom + " " + g.Key.Nom, Count = g.Count() }).ToList()));
				var classement = concours.Classement();
				if (classement != null)
				{
					model.Add(new GenericStatViewModel("StatsNombrePronosticsGagnants", "pronostics gagnés (Phase de poule)", classement.Sum(c => c.NombrePronosGagnes), "panel-success", "fa fa-thumbs-up", classement.OrderByDescending(c => c.NombrePronosGagnes).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombrePronosGagnes}).ToList()));
					model.Add(new GenericStatViewModel("StatsNombrePronosticsPerdus", "pronostics perdus (Phase de poule)", classement.Sum(c => c.NombrePronosPerdus), "panel-danger", "fa fa-thumbs-down", classement.OrderByDescending(c => c.NombrePronosPerdus).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombrePronosPerdus }).ToList()));
					model.Add(new GenericStatViewModel("StatsNombreScoresExacts", "scores exacts (Phase de poule)", classement.Sum(c => c.NombreScoreExact), "panel-success", "fa fa-check", classement.OrderByDescending(c => c.NombreScoreExact).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombreScoreExact }).ToList()));
					model.Add(new GenericStatViewModel("StatsBonnesEquipesQualifieesPoule", "bonnes équipes qualifiées (Phase de poule)", classement.Sum(c => c.NombreBonneEquipeQualifiee), "panel-info", "fa fa-trophy", classement.OrderByDescending(c => c.NombreBonneEquipeQualifiee).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombreBonneEquipeQualifiee }).ToList()));
					model.Add(new GenericStatViewModel("StatsNombrePoulesCompletes", "poules complètes", classement.Sum(c => c.NombrePouleComplete), "panel-info", "fa fa-trophy", classement.OrderByDescending(c => c.NombrePouleComplete).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombrePouleComplete }).ToList()));
					model.Add(new GenericStatViewModel("StatsNombrePronosticsGagnantsNouveauProno", "pronostics gagnés (Phase finale - Nouveau)", classement.Sum(c => c.NombrePronosGagnesNouveauProno), "panel-success", "fa fa-thumbs-up", classement.OrderByDescending(c => c.NombrePronosGagnesNouveauProno).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombrePronosGagnesNouveauProno }).ToList()));
					model.Add(new GenericStatViewModel("StatsNombrePronosticsPerdusNouveauProno", "pronostics perdus (Phase finale - Nouveau)", classement.Sum(c => c.NombrePronosPerdusNouveauProno), "panel-danger", "fa fa-thumbs-down", classement.OrderByDescending(c => c.NombrePronosPerdusNouveauProno).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombrePronosPerdusNouveauProno }).ToList()));
					model.Add(new GenericStatViewModel("StatsNombrePronosticsScoresExactsNouveauProno", "scores exacts (Phase finale - Nouveau)", classement.Sum(c => c.NombreScoreExactNouveauProno), "panel-success", "fa fa-check", classement.OrderByDescending(c => c.NombreScoreExactNouveauProno).Select(s => new GenericListItemViewModel() { Label = s.NomComplet, Count = s.NombreScoreExactNouveauProno }).ToList()));
				}
			}
			
			return View(model);
		}*/
	}
}
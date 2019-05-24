import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSmile } from '@fortawesome/free-solid-svg-icons';
import { faSadTear } from '@fortawesome/free-solid-svg-icons';
import { faAngry } from '@fortawesome/free-solid-svg-icons';



class CreateItem extends Component {
  constructor() {
    super();
    this.state = {
      value: 'Vul hier de titel in'
    }
  }

  changeState(event) {
    console.log("qweqwe");
    this.setState({
      value: event.target.value
    })
  }

    render() {
      return (
      <div className="container-fluid containerAddItem">
        <div className="row">

          <div className="col s5 inputFields">
<p></p>
            <form>
            <div className="row">
              <div className="input-field col s11">
                <input id="title" className="inputField" type="text" onChange={this.changeState.bind(this)}></input>
                <label htmlFor="last_name">Titel</label>
              </div>
              </div>
              <div className="row">
                <div className="input-field col s11">
                  <input id="last_name" className="inputField" type="text"></input>
                  <label htmlFor="last_name">Beschrijving</label>
                </div>
              </div>

              <label htmlFor="last_name">Bestaand document</label>
              <div className="row">
                <div className="switch existingFile">
                  <label>
                    Nee
                    <input type="checkbox"></input>
                    <span className="lever"></span>
                    Ja
                  </label>
                </div>
              </div>

              <div className="row">
                <div className="input-field col s11">
                  <div className="file-field input-field">
                    <div className="btn">
                      <span>Bestand</span>
                      <input type="file"></input>
                    </div>
                    <div className="file-path-wrapper">
                      <input className="file-path validate" type="text" placeholder="Upload hier het gewenste bestand"></input>
                    </div>
                  </div>
                </div>
              </div>

                <label htmlFor="last_name">Quiz</label>
                <div className="row">
                  <div className="switch quiz">
                    <label>
                      Nee
                      <input type="checkbox"></input>
                      <span className="lever"></span>
                      Ja
                    </label>
                  </div>
                </div>

              <label htmlFor="last_name">Kies een item</label>
              <div className="row col s12 itemCollection">

              <label htmlFor="item1">
                  <div className="item">
                    <input className="with-gap" id="item4" name="items" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/rat2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item2">
                  <div className="item">
                    <input className="with-gap" id="item1" name="items" type="radio" defaultChecked></input>
                    <span className="itemImage"><img src={ require('../../../public/images/envelope2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item3">
                  <div className="item">
                    <input className="with-gap" id="item2" name="items" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/candle2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item4">
                  <div className="item">
                    <input className="with-gap" id="item3" name="items" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/binocular2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item5">
                  <div className="item">
                    <input className="with-gap" id="item3" name="items" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/kroontjesPen2.png') } /></span>
                  </div>
                </label>

              </div>

              <div className="row">
                <div className="input-field col s4">
                  <button className="btn waves-effect waves-light cancelButton" type="submit" name="action">Annuleren</button>
                </div>
                <div className="input-field col s4">
                  <button className="btn waves-effect waves-light saveButton" type="submit" name="action">Opslaan</button>
                </div>

              </div>

            </form>
          </div>



          <div className="col s7 example">

            <div className="col s12 background">
            
              <div className="col s12">
              <div className=" exampleCard">
              <div className="all">
                  <div className="title">
                      <p> {this.state.value} </p>
                  </div>
                  <div className="content">
                  <p>Voor het project moet er onderzoek gedaan worden naar het ontwikkelen van een applicatie die er voor zorgt dat bezoekers van het museum via een touchscreen tafel kennis op kunnen doen over de tweede wereldoorlog. Om dit te doen is het belangrijk dat er onderzoek gedaan wordt naar wat museum de Duikelaar precies wil. Daarnaast wordt er ook onderzoek gedaan naar kennismanagement zodat het systeem dat gebouwd wordt overeenkomt met de eisen van de klant.
                      Om de resultaten van het onderzoek en de aanpak van het project vast te leggen wordt alle informatie beschreven in een aantal documenten, namelijk:
                      Om de applicatie goed te kunnen realiseren zijn er een aantal eisen opgesteld door de opdrachtgever. Deze eisen zorgen ervoor dat het doel van een correct werkend spel behaald kan worden.
                      Gedurende het project moet er gewerkt worden via de SCRUM methodiek. Dit houdt in dat er acht sprints van één week zijn waarbij aan het eind van elke sprint een tussenproduct wordt gepresenteerd aan de opdrachtgever;
                      Er moet gebruik gemaakt worden van de cloud;
                      De applicatie moet op de touchscreen tafel, die bij het museum aanwezig is, gespeeld kunnen worden. Dit houdt in dat er touch ondersteuning moet zijn;
                      De code van de applicatie moet voldoen aan de code conventies van NHL Stenden.
                      3.3 Eisen scriptieAan het eind van de projectperiode moet er een scriptie van 18.000 woorden opgesteld worden waarin het project in detail beschreven wordt. Deze moet voldoen aan de eisen die op
                  </p>
                  </div>
                  <div className="quiz">
                      <div className="quizQuestion1">
                      </div>
                      <div className="quizQuestion2">
                      </div>
                      <div className="quizQuestion3">
                      </div>
                      <div className="quizQuestion4">
                      </div>
                  </div>

                  <div className="feedback">
                  <div className="emoOne">
                      <div className="col s2 emoteIconExample">
                          <FontAwesomeIcon icon={faSmile} />
                      </div>
                  </div>
                  <div className="emoTwo">
                      <div className="col s2 emoteIconExample">
                          <FontAwesomeIcon icon={faSadTear} />
                      </div>
                  </div>
                  <div className="emoThree">
                      <div className="col s2 emoteIconExample">
                          <FontAwesomeIcon icon={faAngry} />
                      </div>
                  </div>

                  </div>
                  </div>

              </div>
              </div>
          </div>
          </div>

        </div>
      </div>
      )
    }
}

export default CreateItem;
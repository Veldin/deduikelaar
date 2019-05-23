import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSmile } from '@fortawesome/free-solid-svg-icons';
import { faSadTear } from '@fortawesome/free-solid-svg-icons';
import { faAngry } from '@fortawesome/free-solid-svg-icons';

const example = (props) => {
    return (   
        <div className="col s12 background">
            <div className="col s12 exampleCard">
                <div className="title">
                    {/* <p> {this.state.title} </p> */}
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

    )
};

export default example;
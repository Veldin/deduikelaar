import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';
import Popup from './example/Example';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSmile } from '@fortawesome/free-solid-svg-icons';
import { faSadTear } from '@fortawesome/free-solid-svg-icons';
import { faAngry } from '@fortawesome/free-solid-svg-icons';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';



class CreateItem extends Component {
  constructor() {
    super();
    this.state = {
      title: 'Vul hier de titel in',
      textArea: 'Vul hier de tekst in',
      isChecked: true,
      showPopup: false
    }
  }

  changeState(event) {
    this.setState({
      title: event.target.value
    })
  }
  changeStateTextArea(event) {
    this.setState({
      textArea: event.target.value
    })
  }

  changeStateSwitch() {
    this.setState({
      isChecked: !this.state.isChecked
    });
  }

  togglePopup() {
    this.setState({
      showPopup: !this.state.showPopup
    });

  createItem(e) {
    e.preventDefault();
    fetch('/api/v1/story/',{
      method: 'PUT',
    })
    .then(response => response.json())
    .then(response => {
      console.log('hi: '+response['response']);

      // if(response['response'] == "success"){
      //   toastr.success('Het item is verwijderd!')
      // }else{
      //   toastr.warning('Er is iets fout gedaan. Probeer het a.u.b opnieuw.')
      // }
    })
  }

    render() {
      const isChecked = {
        display: this.state.isChecked ? "block" : "none"
      };
      
      const hidden = {
        display: this.state.isChecked ? "none" : "block"
      }

      return (
      <div className="container-fluid containerAddItem">
        <div className="row">

          <div className="col s5 inputFields">
            <form name="createItemForm">

            <div className="row">
              <div className="input-field col s11">
                <input id="title" name="title" className="inputField" type="text" required onChange={this.changeState.bind(this)}></input>
                <label htmlFor="title">Titel</label>
              </div>
              <div className="question col s1">
                <FontAwesomeIcon icon={ faQuestionCircle } onClick={this.togglePopup.bind(this)} />
              </div>
            </div>

            {this.state.showPopup ?  
                  <Popup  
                    text='Click "Close Button" to hide popup'  
                    closePopup={this.togglePopup.bind(this)}  
                  />  
                  : null  
                  }  

              <label htmlFor="existingFile">Bestaand document</label>
              <div className="row">
                <div className="switch existingFile col s11">
                  <label>
                    Nee
                    <input type="checkbox" name="existingFile" onChange={ this.changeStateSwitch.bind(this) } checked={ this.state.isChecked }></input>
                    <span className="lever"></span>
                    Ja
                  </label>
                </div>
                <div className="question col s1">
                  <FontAwesomeIcon icon={ faQuestionCircle } />
                </div>
              </div>

              <div className="row" style={ hidden }>
                <div className="input-field col s11">
                  <textarea id="textarea1" name="ownText" className="inputField materialize-textarea" onChange={this.changeStateTextArea.bind(this)}></textarea>
                  <label htmlFor="ownText">Eigen tekst</label>
                </div>
                <div className="question col s1">
                  <FontAwesomeIcon icon={ faQuestionCircle } />
                </div>
              </div>

              <div className="row" style={ isChecked }>
                <div className="input-field col s11">
                  <div className="file-field input-field">
                    <div className="btn">
                      <span>Bestand</span>
                      <input name="uploadFileButton" type="file"></input>
                    </div>
                    <div className="file-path-wrapper">
                      <input className="file-path validate" name="uploadFile" type="text" placeholder="Upload hier het gewenste bestand"></input>
                    </div>
                  </div>
                </div>
                <div className="question col s1">
                  <FontAwesomeIcon icon={ faQuestionCircle } />
                </div>
              </div>

              <label htmlFor="chooseIcon">Kies een icoon</label>
              <div className="row col s12 itemCollection">

              <label htmlFor="item1">
                <div className="item">
                  <input className="with-gap" id="item1" name="item1" type="radio" defaultChecked></input>
                  <span className="itemImage"><img src={ require('../../../public/images/rat2.png') } /></span>
                </div>
              </label>

              <label htmlFor="item2">
                <div className="item">
                  <input className="with-gap" id="item2" name="item2" type="radio"></input>
                  <span className="itemImage"><img src={ require('../../../public/images/envelope2.png') } /></span>
                </div>
              </label>

                <label htmlFor="item3">
                  <div className="item">
                    <input className="with-gap" id="item3" name="item3" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/candle2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item4">
                  <div className="item">
                    <input className="with-gap" id="item4" name="item4" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/binocular2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item5">
                  <div className="item">
                    <input className="with-gap" id="item5" name="item5" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/kroontjesPen2.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item6">
                  <div className="item">
                    <input className="with-gap" id="item5" name="item6" type="radio"></input>
                    <span className="itemImage"><img src={ require('../../../public/images/kroontjesPen2.png') } /></span>
                  </div>
                </label>

              </div>

              <div className="row">
                <div className="input-field col s4">
                  <button className="btn waves-effect waves-light saveButton" type="submit" onClick={this.createItem.bind(this)} name="action">Opslaan</button>
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
                        <p> {this.state.title} </p>
                      </div>
                      <div className="content">
                        <p> {this.state.textArea} </p>
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

                      <p>Welke emotie wekte dit verhaal bij jou op?</p>
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
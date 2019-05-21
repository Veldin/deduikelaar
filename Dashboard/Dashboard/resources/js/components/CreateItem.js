import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';
import Example from './example/Example';

class CreateItem extends Component {
    render() {
      return (
      <div className="container-fluid containerAddItem">
        <div className="row">

          <div className="col s5 inputFields">

            <form>
            <div className="row">
              <div className="input-field col s11">
                <input id="last_name" type="text"></input>
                <label htmlFor="last_name">Titel</label>
              </div>
              </div>
              <div className="row">
                <div className="input-field col s11">
                  <input id="last_name" type="text"></input>
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
                    <input className="with-gap" id="item1" name="items" type="radio"></input>
                    <span><img src={ require('./logotest.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item2">
                  <div className="item">
                    <input className="with-gap" id="item2" name="items" type="radio" defaultChecked></input>
                    <span><img src={ require('./logotest.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item3">
                  <div className="item">
                    <input className="with-gap" id="item3" name="items" type="radio"></input>
                    <span><img src={ require('./logotest.png') } /></span>
                  </div>
                </label>

                <label htmlFor="item4">
                  <div className="item">
                    <input className="with-gap" id="item4" name="items" type="radio"></input>
                    <span><img src={ require('./logotest.png') } /></span>
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

              <Example title="De brief van Karel" content="1" />

          </div>

        </div>
      </div>
      )
    }
}

export default CreateItem;
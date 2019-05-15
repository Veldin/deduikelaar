import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class CreateItem extends Component {
    render() {
      return (
      <div className="container-fluid">
        <div className="row">

          <div className="col s5 inputFields">
            <form>
            <div className="row">
              <div className="input-field col s11">
                <input id="last_name" type="text"></input>
                <label for="last_name">Titel</label>
              </div>
              </div>
              <div className="row">
                <div className="input-field col s11">
                  <input id="last_name" type="text"></input>
                  <label for="last_name">Beschrijving</label>
                </div>
              </div>

              <div className="row">
                <div className="switch">
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
                    <span>Bestand downloaden</span>
                    <input type="file"></input>
                  </div>
                  <div className="file-path-wrapper">
                    <input className="file-path validate" type="text"></input>
                  </div>
                </div>
              </div>
              </div>
            </form>
          </div>



          <div className="col s7 inputFields">

            <div className="example">

              <p>hoi dit is een voorbeeld</p>

            </div>

          </div>
        </div>
      </div>
      )
    }
}

export default CreateItem;
import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';
import Card from './Card/Card';

class Overview extends Component {
  componentDidMount() {
    fetch('/api/v1/overview')
      .then(response => response.json())
      .then(json => console.log(json))
  }

  render(){
    return (
      <div className="overviewContentContainer">
        <div className="row overviewFilter">
          <div className="col s2 overviewLabel">Alle Items</div>
          <div className="col s10 overviewSwitch">
            <div className="switch">
              <label>
                Alleen Actief
                <input type="checkbox" defaultChecked></input>
                <span className="lever"></span>
                Alle
              </label>
            </div>
          </div>
        </div>
        <div className="row overviewCards">
          <div className="cards-container">
            <Card title="De brief van Karel" active="1" emoteOne="4" emoteTwo="2" emoteThree="10" />
            <Card title="De brief van Karel" active="1" emoteOne="2" emoteTwo="888" emoteThree="5" />
            <Card title="De brief van Karel" active="1" emoteOne="5" emoteTwo="1" emoteThree="7" />
            <Card title="De brief van Karel" active="1" emoteOne="5" emoteTwo="1" emoteThree="7" />
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;
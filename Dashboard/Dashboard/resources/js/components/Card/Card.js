import React from 'react';
import HSBar from 'react-horizontal-stacked-bar-chart';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'
import { faSmile } from '@fortawesome/free-solid-svg-icons'
import { faSadTear } from '@fortawesome/free-solid-svg-icons'
import { faAngry } from '@fortawesome/free-solid-svg-icons'

const card = (props) => {
  const total = parseInt(props.emoteOne) + parseInt(props.emoteTwo) + parseInt(props.emoteThree);

  const percentageEmoteOne = Math.round((props.emoteOne / total) * 100);
  const percentageEmoteTwo = Math.round((props.emoteTwo / total) * 100);
  const percentageEmoteThree = Math.round((props.emoteThree / total) * 100);

  const percentageEmoteOneDOM = {
    width: percentageEmoteOne
  };

  const percentageEmoteTwoDOM = {
    width: percentageEmoteTwo
  };

  const percentageEmoteThreeDOM = {
    width: percentageEmoteThree
  };

  function deleteItem(e) {
    e.preventDefault();
  }
  
  return (        
    <div className="col s12 m4">
      <div className="card">
         <div className="card-content white-text">
          <div className="row">
            <div className="col s10">
              <div className="switch">
                <label>
                  Actief
                  <input type="checkbox" defaultChecked></input> 
                  <span className="lever"></span>
                  Inactief
                </label>
              </div>
            </div>
            <div className="col s2">
              <div className="deleteItem" onClick={deleteItem}>
                <FontAwesomeIcon icon={faTrashAlt} />
              </div>
            </div>
          </div>
          <div className="row">
            <div className="col s12">
               <span className="card-title">{props.title}</span>
            </div>
          </div>
          <div className="row feedback">
            <div className="col s3">
              <p>Gevoel:</p>
            </div>
            <div className="col s9">
              <div className="bar">
                <HSBar
                  data={[
                    { value: percentageEmoteOne, description: percentageEmoteOne, color: "#ff0043" },
                    { value: percentageEmoteTwo, description: percentageEmoteTwo, color: "#77c6a0" },
                    { value: percentageEmoteThree, description: percentageEmoteThree, color: "#d1d33d" }
                  ]}
                />
              </div>
            </div>
          </div>
         </div>
      </div>
    </div>
  )


};

export default card;
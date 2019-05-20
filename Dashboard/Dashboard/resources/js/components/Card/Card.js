import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'
import { faSmile } from '@fortawesome/free-solid-svg-icons'
import { faSadTear } from '@fortawesome/free-solid-svg-icons'
import { faAngry } from '@fortawesome/free-solid-svg-icons'

const card = (props) => {
  let total = parseInt(props.emoteOne) + parseInt(props.emoteTwo) + parseInt(props.emoteThree);

  let percentageEmoteOne = Math.round((props.emoteOne / total) * 100) +'%';
  let percentageEmoteTwo = Math.round((props.emoteTwo / total) * 100) +'%';
  let percentageEmoteThree = Math.round((props.emoteThree / total) * 100) +'%';

  let percentageEmoteOneDOM = {
    width: percentageEmoteOne
  };

  let percentageEmoteTwoDOM = {
    width: percentageEmoteTwo
  };

  let percentageEmoteThreeDOM = {
    width: percentageEmoteThree
  };

  return (        
    <div className="col s12 m4">
      <div className="card">
         <div className="card-content white-text">
          <div className="row">
            <div className="col s10">
              <div className="switch">
                <label>
                  Actief
                  <input type="checkbox" checked></input> 
                  <span className="lever"></span>
                  Inactief
                </label>
              </div>
            </div>
            <div className="col s2">
              <div className="deleteItem">
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
            <div className="col s9">
              <div className="progress positiveFeedbackBar">
                  <div className="determinate" style={percentageEmoteOneDOM}></div>
              </div>
            </div>
            <div className="col s2">
              <p>{props.emoteOne}</p>
            </div>
            <div className="col s1 emoteIcon">
              <p><FontAwesomeIcon icon={faSmile} /></p>
            </div>
          </div>
          <div className="row feedback">
            <div className="col s9">
              <div className="progress neutralFeedbackBar">
                  <div className="determinate" style={percentageEmoteTwoDOM}></div>
              </div>
            </div>
            <div className="col s2">
              <p>{props.emoteTwo}</p>
            </div>
            <div className="col s1 emoteIcon">
              <p><FontAwesomeIcon icon={faSadTear} /></p>
            </div>
          </div>
          <div className="row feedback">
            <div className="col s9">
              <div className="progress negativeFeedbackBar">
                  <div className="determinate" style={percentageEmoteThreeDOM}></div>
              </div>
            </div>
            <div className="col s2">
              <p>{props.emoteThree}</p>
            </div>
            <div className="col s1 emoteIcon">
              <p><FontAwesomeIcon icon={faAngry} /></p>
            </div>
          </div>
         </div>
      </div>
    </div>
  )


};

export default card;
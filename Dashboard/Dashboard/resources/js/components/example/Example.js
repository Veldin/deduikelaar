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
                </div>
                <div className="content">
                </div>
                <div className="title">
                </div>

                <div className="feedback">
                <div className="emoOne">
                    <div className="col s2 emoteIconExample">
                        <p><FontAwesomeIcon icon={faSmile} /></p>
                    </div>
                </div>
                <div className="emoTwo">
                    <div className="col s2 emoteIconExample">
                        <p><FontAwesomeIcon icon={faSadTear} /></p>
                    </div>
                </div>
                <div className="emoThree">
                    <div className="col s2 emoteIconExample">
                        <p><FontAwesomeIcon icon={faAngry} /></p>
                    </div>
                </div>

                </div>

            </div>
        </div>

    )
};

export default example;
import React, { Component } from 'react';
import { Router, Route, Link } from 'react-router';

export default class Statistics extends Component {
    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-8 col-md-offset-2">
                        <div className="panel panel-default">
                            <div className="panel-heading">Example Component</div>

                            <div className="panel-body">
                                I am an example component!
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
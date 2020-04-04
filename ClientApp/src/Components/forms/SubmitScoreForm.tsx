import React from 'react';
import { Unit } from '../../types/dotnet-types';
import { UnitIcon } from '../StyleComponents';
import { FieldProps } from './Validation';

export interface SubmitScoreProps {
    fieldAttributes?: FieldProps<SubmitScore>;
    unit: keyof typeof Unit;
}

export interface SubmitScore {
    userName: string;
    boardSlug: string;
    score: number;
}

export const SubmitScoreForm: React.FC<SubmitScoreProps> = ({
    unit,
    fieldAttributes,
}) => (
    <>
        <div className="form-group">
            <label htmlFor="score">Score</label>
            <div className="input-group">
                <div className="input-group-prepend">
                    <span className="input-group-text">
                        <UnitIcon forUnit={Unit[unit]} />
                    </span>
                </div>
                <input
                    {...fieldAttributes?.score.attributes}
                    className="form-control"
                />
                <div className="input-group-append">
                    <span className="input-group-text">{unit}</span>
                </div>
            </div>
        </div>
        {/* <div className="form-group">
            <label htmlFor="score">Video Proof</label>
            <ul className="nav nav-tabs">
                <li className="nav-item">
                    <a
                        id="direct-tab"
                        className="nav-link active"
                        data-toggle="tab"
                        href="#direct"
                    >
                        Direct Upload
                    </a>
                </li>
                <li className="nav-item">
                    <a
                        id="youtube-tab"
                        className="nav-link"
                        data-toggle="tab"
                        href="#youtube"
                    >
                        YouTube
                    </a>
                </li>
            </ul>
            <div className="tab-content">
                <div className="tab-pane fade show active" id="direct">
                    <FileUploader id="video-prood" />
                </div>
                <div className="tab-pane fade show" id="youtube">
                    YouTube
                </div>
            </div>
        </div> */}
    </>
);

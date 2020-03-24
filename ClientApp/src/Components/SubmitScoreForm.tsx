import React from 'react';
import { Unit } from '../types/dotnet-types';
import { UnitIcon, FileUploader } from './StyleComponents';

export interface SubmitScoreProps {
    unit: keyof typeof Unit;
}

export const SubmitScoreForm: React.FC<SubmitScoreProps> = ({ unit }) => (
    <>
        <div className="form-group">
            <label htmlFor="score">Score</label>
            <div className="input-group">
                <div className="input-group-prepend">
                    <span className="input-group-text">
                        <UnitIcon forUnit={Unit[unit]} />
                    </span>
                </div>
                <input type="text" className="form-control" />
                <div className="input-group-append">
                    <span className="input-group-text">{unit}</span>
                </div>
            </div>
        </div>
        <div className="form-group">
            <label htmlFor="score">Video Proof</label>
            <p className="text-muted">
                Don&apos;t want to provide a video? Don&apos;t worry about it!
            </p>
            <input type="text" className="form-control" />
        </div>
    </>
);

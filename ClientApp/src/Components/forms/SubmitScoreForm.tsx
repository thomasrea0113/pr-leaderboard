import React from 'react';
import { Unit } from '../../types/dotnet-types';
import { UnitIcon } from '../StyleComponents';
import {
    FormFieldProps,
    ReactFormFieldProps,
} from '../../types/react-tag-props';
import { validationResultFor } from '../../hooks/useValidation';

export interface SubmitScoreProps {
    fieldAttributes?: FormFieldProps<SubmitScore>;
    unit: keyof typeof Unit;
}

export interface SubmitScore {
    userName: string;
    boardId: string;
    score: number;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const isSubmitScore = (obj: any): obj is SubmitScore => {
    const submitScore = obj as SubmitScore;
    if (
        submitScore != null &&
        typeof submitScore.boardId === 'string' &&
        typeof submitScore.userName === 'string' &&
        typeof submitScore.score === 'number'
    )
        return true;
    return false;
};

const validatorFor = (field?: ReactFormFieldProps) =>
    field != null ? validationResultFor(field) : null;

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
                <input {...fieldAttributes?.score} className="form-control" />
                <div className="input-group-append">
                    <span className="input-group-text">{unit}</span>
                </div>
            </div>
            {validatorFor(fieldAttributes?.score)}
            {validatorFor(fieldAttributes?.boardId)}
            {validatorFor(fieldAttributes?.userName)}
        </div>
        <input {...fieldAttributes?.boardId} type="hidden" />
        <input {...fieldAttributes?.userName} type="hidden" />
    </>
);

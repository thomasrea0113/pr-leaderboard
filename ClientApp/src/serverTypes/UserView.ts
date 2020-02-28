import Leaderboard from './Leaderboard';
import UnitOfMeasure from './UnitOfMeasure';
import Division from './Division';
import WeightClass from './WeightClass';

export default class UserViewModel extends Leaderboard {
    public constructor(
        public isMember: boolean,
        public isRecommended: boolean,
        name: string,
        uom: UnitOfMeasure,
        division: Division,
        weightClass: WeightClass
    ) {
        super(name, uom, division, weightClass);
    }
}

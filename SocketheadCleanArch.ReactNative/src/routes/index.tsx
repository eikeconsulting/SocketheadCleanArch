import { View, Text } from 'react-native';
import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { NavigationContainer } from '@react-navigation/native';
import { Login, Signup } from '@app/screens';

export type RootStackParamsList = {
    Signup: undefined,
    Login: undefined
};

const Stack = createNativeStackNavigator<RootStackParamsList>();

const Routes = () => {
    return (
        <NavigationContainer>
            <Stack.Navigator>
                <Stack.Screen name='Login' component={Login} />
                <Stack.Screen name='Signup' component={Signup} />
            </Stack.Navigator>
        </NavigationContainer>
    );
}

export default Routes;
